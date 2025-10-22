FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["GymTime.Api/GymTime.Api.csproj", "GymTime.Api/"]
COPY ["GymTime.Application/GymTime.Application.csproj", "GymTime.Application/"]
COPY ["GymTime.Infrastructure/GymTime.Infrastructure.csproj", "GymTime.Infrastructure/"]
COPY ["GymTime.Domain/GymTime.Domain.csproj", "GymTime.Domain/"]

RUN dotnet restore "GymTime.Api/GymTime.Api.csproj"

COPY . .
WORKDIR /src/GymTime.Api
RUN dotnet publish "GymTime.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "GymTime.Api.dll"]
