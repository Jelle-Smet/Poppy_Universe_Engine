using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    internal class VisibilityCalculator
    {
        private readonly User_Object user;
        private static readonly HttpClient client = new HttpClient();
        private int hours;

        internal VisibilityCalculator(User_Object u)
        {
            user = u ?? throw new ArgumentNullException(nameof(u));
        }

        /// <summary>
        /// Fetch hourly weather forecast for user's location with wind data
        /// </summary>
        public async Task<JsonElement> FetchWeatherAsync(int hours = 12)
        {
            this.hours = hours;
            try
            {
                string url = $"https://api.open-meteo.com/v1/forecast?latitude={user.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={user.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&hourly=cloudcover,precipitation,windspeed_10m&timezone=auto";
                var response = await client.GetStringAsync(url);
                return JsonSerializer.Deserialize<JsonElement>(response)!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to fetch weather data.", ex);
            }
        }

        /// <summary>
        /// Calculate weather-based visibility chance (0-100%) with improved accuracy
        /// </summary>
        public (double Chance, string Reason) ComputeWeatherChanceWithReason(
            JsonElement weatherData,
            
            int cloudThreshold = 30)
        {
            var hourly = weatherData.GetProperty("hourly");

            var cloudArray = hourly.GetProperty("cloudcover").EnumerateArray();
            var precipArray = hourly.GetProperty("precipitation").EnumerateArray();

            // === WIND FIX ===
            // Always prepare a non-null enumerator + a flag
            JsonElement.ArrayEnumerator windEnum = default;
            bool hasWind = false;

            if (hourly.TryGetProperty("windspeed_10m", out var windProperty))
            {
                windEnum = windProperty.EnumerateArray();
                hasWind = true;
            }

            int hoursCloudy = 0;
            int hoursLightRain = 0;
            int hoursHeavyRain = 0;
            int hoursHighWind = 0;

            double totalScore = 0;
            double totalWeight = 0;

            double maxPrecip = 0;
            double avgCloud = 0;
            int cloudCount = 0;

            using (var cEnum = cloudArray.GetEnumerator())
            using (var pEnum = precipArray.GetEnumerator())
            using (var wEnum = windEnum) // always valid, even if default
            {
                int i = 0;

                while (cEnum.MoveNext() && pEnum.MoveNext() && i < hours)
                {
                    double cloud = cEnum.Current.GetDouble();
                    double precip = pEnum.Current.GetDouble();

                    // SAFE wind read
                    double windSpeed = 0;
                    if (hasWind && wEnum.MoveNext())
                    {
                        windSpeed = wEnum.Current.GetDouble();
                    }

                    // --- Track metrics ---
                    avgCloud += cloud;
                    cloudCount++;

                    if (cloud >= cloudThreshold) hoursCloudy++;
                    if (precip > 0 && precip <= 2.5) hoursLightRain++;
                    if (precip > 2.5) hoursHeavyRain++;
                    if (precip > maxPrecip) maxPrecip = precip;
                    if (windSpeed > 30) hoursHighWind++;

                    // --- Time-weighted scoring ---
                    double timeWeight = 1.0 - (i * 0.05);
                    if (timeWeight < 0.5) timeWeight = 0.5;

                    double score = 1.0;

                    // === PRECIPITATION ===
                    if (precip > 0)
                    {
                        if (precip <= 0.5)
                            score *= 0.75;
                        else if (precip <= 2.5)
                            score *= 0.40;
                        else if (precip <= 7.5)
                            score *= 0.15;
                        else
                            score *= 0.05;
                    }

                    // === CLOUDS ===
                    if (cloud > cloudThreshold)
                    {
                        double over = cloud - cloudThreshold;
                        double penalty;

                        if (cloud < 50)
                            penalty = 0.85 - (over / 100.0) * 0.5;
                        else if (cloud < 75)
                            penalty = 0.70 - ((cloud - 50) / 100.0) * 0.4;
                        else
                            penalty = 0.40 - ((cloud - 75) / 100.0) * 0.3;

                        score *= Math.Max(0.1, penalty);
                    }

                    // === WIND ===
                    if (windSpeed > 30)
                    {
                        double windPenalty = Math.Max(0.85, 1.0 - ((windSpeed - 30) / 100.0));
                        score *= windPenalty;
                    }

                    // === COMBO PENALTY ===
                    if (precip > 0 && cloud > 60)
                        score *= 0.85;

                    // clamp
                    score = Math.Max(0.0, Math.Min(1.0, score));

                    totalScore += score * timeWeight;
                    totalWeight += timeWeight;

                    i++;
                }
            }

            // final values
            double chance = totalWeight == 0 ? 0 : (totalScore / totalWeight) * 100.0;
            avgCloud = cloudCount > 0 ? avgCloud / cloudCount : 0;

            string reason = GenerateDetailedReason(
                hours,
                hoursCloudy,
                hoursLightRain,
                hoursHeavyRain,
                hoursHighWind,
                maxPrecip,
                avgCloud,
                chance
            );

            return (chance, reason);
        }
        private string GenerateDetailedReason(
            int totalHours,
            int hoursCloudy,
            int hoursLightRain,
            int hoursHeavyRain,
            int hoursHighWind,
            double maxPrecip,
            double avgCloud,
            double visibilityChance)
        {
            int totalRainHours = hoursLightRain + hoursHeavyRain;

            // Excellent conditions
            if (visibilityChance >= 85)
            {
                if (avgCloud < 20)
                    return "Exceptional viewing conditions! Crystal clear skies expected.";
                else
                    return "Great visibility expected with mostly clear skies.";
            }

            // Good conditions
            if (visibilityChance >= 70)
            {
                if (hoursCloudy > 0)
                    return $"Good conditions overall, though some clouds ({hoursCloudy}h) may pass through.";
                else
                    return "Good visibility expected with favorable weather.";
            }

            // Fair conditions
            if (visibilityChance >= 50)
            {
                if (totalRainHours > 0 && hoursCloudy > totalHours * 0.5)
                    return $"Mixed conditions with {totalRainHours}h of rain and {hoursCloudy}h of clouds. Visibility will be intermittent.";
                else if (totalRainHours > 0)
                    return $"Fair conditions, but expect some rain ({totalRainHours}h) that will temporarily reduce visibility.";
                else
                    return $"Partly cloudy skies ({hoursCloudy}h) will create variable viewing conditions.";
            }

            // Poor conditions
            if (visibilityChance >= 30)
            {
                if (hoursHeavyRain > 0)
                    return $"Challenging conditions with {hoursHeavyRain}h of heavy rain" +
                           (maxPrecip > 7.5 ? $" (up to {maxPrecip:F1}mm/h)" : "") +
                           ". Visibility will be significantly reduced.";
                else if (totalRainHours >= totalHours * 0.6)
                    return $"Poor visibility likely due to persistent rain ({totalRainHours}h) throughout the period.";
                else
                    return $"Difficult viewing conditions with extensive cloud cover ({hoursCloudy}h) and some precipitation.";
            }

            // Very poor conditions
            if (hoursHeavyRain >= totalHours * 0.4)
                return $"Very poor conditions. Heavy rain expected for {hoursHeavyRain}h with storms possible (max {maxPrecip:F1}mm/h). Visibility will be minimal.";
            else if (totalRainHours >= totalHours * 0.7 && hoursCloudy >= totalHours * 0.8)
                return $"Unfavorable weather: near-constant rain ({totalRainHours}h) and thick cloud cover ({hoursCloudy}h). Visibility will be severely limited.";
            else if (avgCloud > 85)
                return $"Very cloudy conditions (avg {avgCloud:F0}% cover) will severely obstruct visibility.";
            else
                return $"Poor conditions with {totalRainHours}h of rain and {hoursCloudy}h of heavy clouds. Not ideal for observation.";
        }
    }
}