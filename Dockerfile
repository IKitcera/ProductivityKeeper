## Stage 1: Build frontend (Angular)
#FROM node:latest AS frontend-builder
#WORKDIR /app/ProductivityKeeperClient
#COPY ProductivityKeeperClient/package.json ProductivityKeeperClient/package-lock.json ./
#RUN npm install --force
#COPY ProductivityKeeperClient .
#RUN npm run build

# Stage 2: Build backend (ASP.NET Core)

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
WORKDIR /src
COPY ["ProductivityKeeperWeb/ProductivityKeeperWeb.csproj", "ProductivityKeeperWeb/"]
COPY ["UserTasksProgressPredictionEngine/UserTasksProgressPredictionEngine.csproj", "UserTasksProgressPredictionEngine/"]
RUN dotnet restore "ProductivityKeeperWeb/ProductivityKeeperWeb.csproj"
COPY . .
WORKDIR "/src/ProductivityKeeperWeb"
RUN dotnet build "ProductivityKeeperWeb.csproj" -c Release -o /app/publish


# Stage 5: Merge frontend, backend, and prediction engine into final image
FROM base AS final
WORKDIR /app
#COPY --from=frontend-builder /app/ProductivityKeeperClient/dist ./ProductivityKeeperClient/wwwroot
COPY --from=backend-builder /app/publish .
ENTRYPOINT ["dotnet", "ProductivityKeeperWeb/ProductivityKeeperWeb.dll"]
