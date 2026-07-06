# Overview

Welcome on this website which has two goals: show my professional assets and demonstrate some of my skills.

# Architecture

```
┌─────────────────────────────────┐     ┌──────────────────────────────┐
│   Azure Static Web App          │     │   Azure Function App         │
│   Blazor WASM (.NET 8)          │────▶│   MyCv.Api (consumption)     │
│   MudBlazor UI                  │     │   /api/tailor                │
└─────────────────────────────────┘     └──────────────┬───────────────┘
                                                        │
                                                        ▼
                                            ┌───────────────────────┐
                                            │   Google Gemini API   │
                                            │   gemini-1.5-flash    │
                                            └───────────────────────┘
```

# Features

## AI CV Tailor
The hero section includes an AI-powered search bar where a recruiter can paste a job title or description.
The result is analyzed by Gemini and returns a structured response in 3 cases:

| Case | Description | Stars |
|------|-------------|-------|
| 1 | Not relevant | 0 ★ |
| 2 | Partial match — shows skill bridges (e.g. AWS → Azure) and transferable skills | 1–3 ★ |
| 3 | Strong match — pitch, matching skills, bonus skills the recruiter didn't think of | 4–5 ★ |

The service is fully decoupled from the rest of the page — no Blazor cascading, no DOM coupling.
For local development a `FakeTailorService` is used (see [Local development](#local-development)).

# Hosting

## Azure

The website is deployed on Azure as Static Web App for two environments:

**Production**
- https://icy-grass-0f365a003.7.azurestaticapps.net
- https://www.valerian-verona.fr/

**Development**
- https://salmon-sand-05c416103.7.azurestaticapps.net

> The website was previously a Blazor server app and deployed as Azure Web App
> - Production: https://valerianverona-degvfed0djcrcfen.francecentral-01.azurewebsites.net
> - Development: https://valerianverona-dev-fmd7cvg2b8dscrdf.francecentral-01.azurewebsites.net

## Azure Function App

The AI Tailor feature is backed by a separate Azure Function App (consumption plan, free tier).

**Endpoints**
- `POST /api/tailor` — accepts `{ "input": "job description" }`, returns structured JSON

**Configuration**
The following application settings must be set in the Azure Portal:

| Setting | Description |
|---------|-------------|
| `GEMINI_API_KEY` | Google Gemini API key from [aistudio.google.com](https://aistudio.google.com) |

**CORS**
`https://www.valerian-verona.fr` must be added to the Function App's allowed origins.
`https://localhost:7064` is also added for local testing purpose (HTTPS profile in `launchsettings.json`).

## Domain

The domain `valerian-verona.fr` is owned by [OVH Cloud](https://manager.eu.ovhcloud.com/#/hub/).

**Configuration**
Added to custom domains of the `MyCv-prod` Static Web App using CName (see "DNS Zone" in OVH Cloud).

# Local Development

## Prerequisites
- .NET 8 SDK
- [Azure Functions Core Tools v4](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)
  ```bash
  npm install -g azure-functions-core-tools@4
  ```

## Run the Blazor app

```bash
cd ui/MyCv.UI.Wasm
dotnet watch run
```

The app runs on `https://localhost:5001`.
In `DEBUG` mode, `FakeTailorService` is used automatically — no API key or Function App needed.

## Run the Function App locally (optional, for real AI testing)

1. Set your Gemini API key in `ui/MyCv.Api/local.settings.json`:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "GEMINI_API_KEY": "YOUR_KEY_HERE"
  }
}
```

2. Start the Function App:
```bash
cd ui/MyCv.Api
func start
```

3. In `appsettings.Development.json`, switch to the real service:
```json
{
  "TailorApi": {
    "Url": "http://localhost:7071/api/tailor",
    "UseFake": false
  }
}
```

# CI/CD

Deployments are handled by GitHub Actions (`.github/workflows/`).

| Trigger | Environment | Version |
|---------|-------------|---------|
| Push to `main` or PR | Development | `dev` |
| Tag `v*` (e.g. `v1.2.0`) | Production | tag name |

Build info (`Version`, `BuildDate`) is injected at build time via `sed` into `BuildInfo.cs` and `sitemap.xml`.