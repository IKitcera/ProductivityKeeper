##See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
##Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
##For more information, please see https://aka.ms/containercompat
#
#FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["ProductivityKeeperWeb/ProductivityKeeperWeb.csproj", "ProductivityKeeperWeb/"]
#COPY ["UserTasksProgressPredictionEngine/UserTasksProgressPredictionEngine.csproj", "UserTasksProgressPredictionEngine/"]
#RUN dotnet restore "./ProductivityKeeperWeb/ProductivityKeeperWeb.csproj"
#COPY . .
#WORKDIR "/src/ProductivityKeeperWeb"
#RUN dotnet build "./ProductivityKeeperWeb.csproj" -c %BUILD_CONFIGURATION% -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./ProductivityKeeperWeb.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "ProductivityKeeperWeb.dll"]

# Stage 1: Build Angular frontend
FROM node:20 AS frontend
WORKDIR /app/ProductivityKeeperClient
COPY ProductivityKeeperClient/package*.json ./
RUN npm install --force
COPY ProductivityKeeperClient ./
RUN npm run build --prod

# Stage 2: Build .NET Core backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend
WORKDIR /app/ProductivityKeeperWeb
COPY ProductivityKeeperWeb/*.csproj ./
RUN dotnet restoreho
COPY ProductivityKeeperWeb ./
RUN dotnet publish -c Release -o out

# Stage 3: Create final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=frontend /app/ProductivityKeeperClient/dist ./wwwroot
COPY --from=backend /app/ProductivityKeeperWeb/out .
ENTRYPOINT ["dotnet", "ProductivityKeeperWeb.dll"]
