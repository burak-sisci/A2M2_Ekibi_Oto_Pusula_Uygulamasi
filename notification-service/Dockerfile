FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# HF Spaces default port 7860 — Program.cs PORT env'inden okur
ENV PORT=7860
EXPOSE 7860

ENTRYPOINT ["dotnet", "NotificationService.dll"]
