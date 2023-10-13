#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Oski.Presentation/Oski.Presentation.csproj", "Oski.Presentation/"]
COPY ["Oski.Application/Oski.Application.csproj", "Oski.Application/"]
COPY ["Oski.Domain/Oski.Domain.csproj", "Oski.Domain/"]
COPY ["Oski.Persistance/Oski.Persistance.csproj", "Oski.Persistance/"]
RUN dotnet restore "Oski.Presentation/Oski.Presentation.csproj"
COPY . .
WORKDIR "/src/Oski.Presentation"
RUN dotnet build "Oski.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Oski.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Oski.Presentation.dll"]




 

 
