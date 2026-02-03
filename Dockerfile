# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project and restore
COPY ["Poppy_Universe_Engine/Poppy_Universe_Engine.csproj", "Poppy_Universe_Engine/"]
RUN dotnet restore "Poppy_Universe_Engine/Poppy_Universe_Engine.csproj"

# Copy everything else
COPY . .
WORKDIR "/app/Poppy_Universe_Engine"
RUN dotnet publish "Poppy_Universe_Engine.csproj" -c Release -o /out

# 2. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Poppy_Universe_Engine.dll"]
