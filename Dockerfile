# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 2. Copy the .csproj and restore dependencies
# Based on your structure: Recommendation_Engine (Root) / Poppy_Universe_Engine / .csproj
COPY ["Poppy_Universe_Engine/Poppy_Universe_Engine.csproj", "Poppy_Universe_Engine/"]
RUN dotnet restore "Poppy_Universe_Engine/Poppy_Universe_Engine.csproj"

# 3. Copy everything and build the release
COPY . .
WORKDIR "/app/Poppy_Universe_Engine"
RUN dotnet publish -c Release -o /out

# 4. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# 5. The "Stay Alive" Trick
# Because it's a console app, we tell it to wait for input forever 
# so Render doesn't think the app crashed/finished.
ENTRYPOINT ["dotnet", "Poppy_Universe_Engine.dll"]
CMD ["tail", "-f", "/dev/null"]
