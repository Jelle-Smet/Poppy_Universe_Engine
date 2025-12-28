using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Poppy_Universe_Engine
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 🚀 STEP 1: INITIALIZE WEB SERVER
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Render assigns a port via environment variable
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
            app.Urls.Add($"http://0.0.0.0:{port}");

            // 🛠️ HEALTH CHECK: So Render knows the engine is alive
            app.MapGet("/", () => "🌌 Poppy Universe Engine is Online!");

            // 🧠 THE ENGINE ENDPOINT
            app.MapPost("/run-engine", async (HttpContext context) =>
            {
                try
                {
                    // 1. Read Payload from Request Body
                    using var reader = new StreamReader(context.Request.Body);
                    string jsonInput = await reader.ReadToEndAsync();

                    if (string.IsNullOrWhiteSpace(jsonInput))
                        return Results.BadRequest("Empty payload received.");

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
                    options.Converters.Add(new BoolConverter());
                    options.Converters.Add(new StringConverter());

                    var payload = JsonSerializer.Deserialize<PayloadWrapper>(jsonInput, options);

                    // 2. Extract Data for Engines
                    List<Star_Objects> Stars = payload.Pool.Stars;
                    List<Planet_Objects> Planets = payload.Pool.Planets;
                    List<Moon_Objects> Moons = payload.Pool.Moons;
                    var User = payload.User;
                    DateTime utcTime = User.ObservationTime;

                    // 3. Keep your Console Prints for Render Logs
                    PrintSectionHeader("USER INFORMATION");
                    Console.WriteLine($"┌─ User: {User.Name} | ID: {User.ID}");
                    Console.WriteLine($"│  Position: Lat {User.Latitude:F2} | Long {User.Longitude:F2}");

                    // 4. LAYER 1: PERSONALIZED RECOMMENDATIONS
                    PrintSectionHeader("LAYER 1 PERSONALIZED RESULTS");
                    var starEngine = new Layer1_Star_Engine(User);
                    var planetEngine = new Layer1_Planet_Engine(User);
                    var moonEngine = new Layer1_Moon_Engine(User);

                    var L1_recommendedStars = starEngine.GetRecommendedStars(Stars, utcTime, User) ?? new List<Star_View>();
                    var L1_recommendedPlanets = planetEngine.GetRecommendedPlanets(Planets, utcTime, User) ?? new List<Planet_View>();
                    var L1_recommendedMoons = moonEngine.GetRecommendedMoons(Moons, L1_recommendedPlanets, utcTime) ?? new List<Moon_View>();

                    var finalStars = L1_recommendedStars;
                    var finalPlanets = L1_recommendedPlanets;
                    var finalMoons = L1_recommendedMoons;

                    // 5. LAYER 2: TRENDING
                    var L2_recommendedStars = new List<Star_View>();
                    var L2_recommendedPlanets = new List<Planet_View>();
                    var L2_recommendedMoons = new List<Moon_View>();

                    if (payload.Config != null && payload.Config.L2 && payload.Layer2Data != null)
                    {
                        PrintSectionHeader("LAYER 2 BOOSTING RESULTS");
                        var booster = new Layer_2_Poppys_Trend_Booster();
                        var L2_Results = booster.BoostAll(L1_recommendedStars, L1_recommendedPlanets, L1_recommendedMoons, payload.Layer2Data, topPerType: 10);

                        finalStars = L2_Results.RecommendedStars;
                        finalPlanets = L2_Results.RecommendedPlanets;
                        finalMoons = L2_Results.RecommendedMoons;
                        L2_recommendedStars = finalStars;
                        L2_recommendedPlanets = finalPlanets;
                        L2_recommendedMoons = finalMoons;
                    }

                    // 6. LAYER 3: MATRIX FACTORIZATION
                    var L3_recommendedStars = new List<Star_View>();
                    var L3_recommendedPlanets = new List<Planet_View>();
                    var L3_recommendedMoons = new List<Moon_View>();

                    if (payload.Config != null && payload.Config.L3 && payload.Layer3Data != null)
                    {
                        PrintSectionHeader("LAYER 3 PERSONALIZATION");
                        var L3_personalizer = new Layer_3_Poppys_Matrix_Booster();
                        var L3_Results = L3_personalizer.BoostAll(L1_recommendedStars, L1_recommendedPlanets, L1_recommendedMoons, payload.Layer3Data, topPerType: 10);

                        finalStars = L3_Results.RecommendedStars;
                        finalPlanets = L3_Results.RecommendedPlanets;
                        finalMoons = L3_Results.RecommendedMoons;
                        L3_recommendedStars = finalStars;
                        L3_recommendedPlanets = finalPlanets;
                        L3_recommendedMoons = finalMoons;
                    }

                    // 7. LAYER 4: NEURAL NETWORK
                    var L4_recommendedStars = new List<Star_View>();
                    var L4_recommendedPlanets = new List<Planet_View>();
                    var L4_recommendedMoons = new List<Moon_View>();

                    if (payload.Config.L4 && payload.Layer4Data != null)
                    {
                        PrintSectionHeader("LAYER 4 NEURAL NETWORK BOOST");
                        var L4_personalizer = new Layer_4_Poppys_NN_Booster();
                        var L4_Results = L4_personalizer.BoostAll(L1_recommendedStars, L1_recommendedPlanets, L1_recommendedMoons, payload.Layer4Data, topPerType: 10);

                        finalStars = L4_Results.RecommendedStars;
                        finalPlanets = L4_Results.RecommendedPlanets;
                        finalMoons = L4_Results.RecommendedMoons;
                        L4_recommendedStars = finalStars;
                        L4_recommendedPlanets = finalPlanets;
                        L4_recommendedMoons = finalMoons;
                    }

                    // 8. LAYER 5: GENETIC ALGORITHM RANK FUSION
                    var L5_recommendedStars = new List<Layer5_Poppys_GA_Object>();
                    var L5_recommendedPlanets = new List<Layer5_Poppys_GA_Object>();
                    var L5_recommendedMoons = new List<Layer5_Poppys_GA_Object>();

                    if (payload.Config.L5)
                    {
                        PrintSectionHeader("LAYER 5 GA RANK FUSION");
                        var layer5Handler = new Layer5_Poppys_GA_Handler(seed: 42);
                        var L5_Results = layer5Handler.RunOptimization(
                            user: payload.User,
                            L1_Stars: L1_recommendedStars, L1_Planets: L1_recommendedPlanets, L1_Moons: L1_recommendedMoons,
                            L2_Stars: L2_recommendedStars, L2_Planets: L2_recommendedPlanets, L2_Moons: L2_recommendedMoons,
                            L3_Stars: L3_recommendedStars, L3_Planets: L3_recommendedPlanets, L3_Moons: L3_recommendedMoons,
                            L4_Stars: L4_recommendedStars, L4_Planets: L4_recommendedPlanets, L4_Moons: L4_recommendedMoons
                        );

                        L5_recommendedStars = L5_Results.Stars;
                        L5_recommendedPlanets = L5_Results.Planets;
                        L5_recommendedMoons = L5_Results.Moons;

                        PrintSectionFooter("END OF LAYER 5");
                        return Results.Json(new { Stars = L5_recommendedStars, Planets = L5_recommendedPlanets, Moons = L5_recommendedMoons });
                    }

                    // 9. Return JSON results for L1-L4
                    return Results.Json(new { Stars = finalStars, Planets = finalPlanets, Moons = finalMoons });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Engine Error: {ex.Message}");
                    return Results.Problem(ex.Message);
                }
            });

            await app.RunAsync();
        }

        // ═══════════════════════════════════════════════════════════════
        // Helper Methods for Pretty Console Output
        // ═══════════════════════════════════════════════════════════════

        private static void PrintSectionHeader(string title)
        {
            string border = new string('═', 70);
            string spacedTitle = $"█  {title}  █";
            Console.WriteLine("\n" + border);
            Console.WriteLine(spacedTitle.PadLeft((70 + spacedTitle.Length) / 2).PadRight(70));
            Console.WriteLine(border + "\n");
        }

        private static void PrintSectionFooter(string title)
        {
            string border = new string('═', 70);
            string spacedTitle = $">>> {title} <<<";
            Console.WriteLine("\n" + border);
            Console.WriteLine(spacedTitle.PadLeft((70 + spacedTitle.Length) / 2).PadRight(70));
            Console.WriteLine(border + "\n");
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // Data Classes
    // ═══════════════════════════════════════════════════════════════

    public class EngineConfig { public bool L2 { get; set; } public bool L3 { get; set; } public bool L4 { get; set; } public bool L5 { get; set; } }

    public class PayloadWrapper
    {
        public User_Object User { get; set; }
        public PoolWrapper Pool { get; set; }
        public EngineConfig Config { get; set; }
        public List<Layer2_Interaction_Object> Layer2Data { get; set; }
        public Layer3_User_Matrix_Object Layer3Data { get; set; }
        public Layer4_User_NN_Object Layer4Data { get; set; }
    }

    public class PoolWrapper { public List<Star_Objects> Stars { get; set; } public List<Planet_Objects> Planets { get; set; } public List<Moon_Objects> Moons { get; set; } }

    public class BoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number) return reader.GetInt32() == 1;
            if (reader.TokenType == JsonTokenType.String) return reader.GetString() == "1" || reader.GetString()?.ToLower() == "true";
            return reader.GetBoolean();
        }
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteBooleanValue(value);
    }

    public class StringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number) return reader.TryGetInt64(out long l) ? l.ToString() : reader.GetDouble().ToString();
            return reader.GetString();
        }
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) => writer.WriteStringValue(value);
    }
}