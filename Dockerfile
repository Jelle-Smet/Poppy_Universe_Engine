# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 2. Copy the .csproj and restore dependencies
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

# 5. Expose port so Railway knows this is a web service
EXPOSE 8080

# 6. Start your engine
ENTRYPOINT ["dotnet", "Poppy_Universe_Engine.dll"]
