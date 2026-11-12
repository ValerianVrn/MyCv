# Overview

Welcome on this website which has two goals: show my professional assets and demonstrate some of my skills.

Live: https://www.valerian-verona.fr

> Contributors / AI assistants: start with [CLAUDE.md](CLAUDE.md). More docs in [docs/](docs/):
> [roadmap](docs/ROADMAP.md) · [updating content](docs/CONTENT.md) · [AI Tailor](docs/TAILOR.md)

# Repository layout

| Path | Status | Description |
|------|--------|-------------|
| `ui/MyCv.UI.Wasm` | **Live** | Blazor WebAssembly front-end (.NET 10, MudBlazor), FR/EN |
| `tailor/MyCv.Tailor.Api` | **Live** | Azure Function backing the AI CV Tailor (Gemini) |
| `tailor/MyCv.Tailor.Api.Tests` | **Live** | Unit tests of the Function |
| `ui/MyCv.UI` | Legacy | Previous Blazor Server version of the site |
| `rating/*`, `common/*`, `docker-compose*` | Legacy | CQRS / event-sourcing rating microservices (showcase, not deployed) |

# Architecture

```
┌─────────────────────────────────┐     ┌──────────────────────────────┐
│   Azure Static Web App          │     │   Azure Function App         │
│   Blazor WASM (.NET 10)         │────▶│   MyCv.Tailor.Api (consump.) │
│   MudBlazor UI                  │     │   /api/tailor                │
└─────────────────────────────────┘     └──────────────┬───────────────┘
                                                        │
                                                        ▼
                                            ┌───────────────────────┐
                                            │   Google Gemini API   │
                                            │   gemini-2.5-flash    │
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
For local development a `FakeTailorService` can be used (see [Local development](#local-development)).
Request/response contract, error codes and limits: see [docs/TAILOR.md](docs/TAILOR.md).

# Hosting

## Azure

The website is deployed on Azure as Static Web App for two environments:

**Production**
- https://icy-grass-0f365a003.7.azurestaticapps.net
- https://www.valerian-verona.fr/

**Development**
- https://salmon-sand-05c416103.7.azurestaticapps.net

> The website was previously a Blazor Server app (`ui/MyCv.UI`) deployed as Azure Web Apps.
> Those Web Apps and their deploy workflows have been decommissioned.

## Azure Function App

The AI Tailor feature is backed by a separate Azure Function App (consumption plan, free tier).

**Endpoints**
- `POST /api/tailor` — accepts `{ "input": "job description" }` (max 4000 characters), returns structured JSON

**Configuration**
The following application settings must be set in the Azure Portal:

| Setting | Description |
|---------|-------------|
| `GEMINI_API_KEY` | Google Gemini API key from [aistudio.google.com](https://aistudio.google.com) |
| `GEMINI_MODEL` | Optional. Gemini model name, default `gemini-2.5-flash`. Lets you switch models without redeploying |
| `ALLOWED_ORIGINS` | Optional, comma-separated. Code-level CORS fallback, **only used when platform CORS is off** (e.g. `func start`). Default: `https://www.valerian-verona.fr` |

**Runtime**
The Function App stack must be **.NET 10 (isolated worker)** (Azure Portal → Function App → Configuration → General settings).

**CORS**
**Platform CORS** (Function App → API → CORS) is the source of truth. It answers preflights before the code runs and
takes precedence over the headers set in code. Allowed origins:

| Origin | Why |
|--------|-----|
| `https://www.valerian-verona.fr` | Production site |
| `https://salmon-sand-05c416103.7.azurestaticapps.net` | Development site (`develop`) |
| `https://localhost:7064` | Local front-end (HTTPS profile in `launchSettings.json`) |
| `https://portal.azure.com` | Testing from the Azure Portal |

Never add `*`. PR preview environments (`…-<number>.westeurope…`) change URL for every PR, so they are not listed
and the Tailor doesn't work on them. Dev also calls the **production** Function, because the Function only deploys from `main`.

```bash
az functionapp cors show -g MyCV -n mycv-api
az functionapp cors add  -g MyCV -n mycv-api --allowed-origins https://example.com
```

## Domain

The domain `valerian-verona.fr` is owned by [OVH Cloud](https://manager.eu.ovhcloud.com/#/hub/).

**Configuration**
Added to custom domains of the `MyCv-prod` Static Web App using CName (see "DNS Zone" in OVH Cloud).

# Local Development

## Prerequisites
- .NET 10 SDK
- [Azure Functions Core Tools v4](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)
  ```bash
  npm install -g azure-functions-core-tools@4
  ```

## Run the Blazor app

```bash
cd ui/MyCv.UI.Wasm
dotnet watch run
```

The app URL is printed by `dotnet watch` (see `Properties/launchSettings.json`).

The Tailor backend is selected by `TailorApi:UseFake` in `wwwroot/appsettings.Development.json`:
- `true` → `FakeTailorService` (no API key or Function App needed)
- `false` → real HTTP call to `TailorApi:Url`. It is **not** overridden in development, so it points to the
  production Function from `wwwroot/appsettings.json` unless you set it (see step 3 below)

## Run the Function App locally (optional, for real AI testing)

1. Set your Gemini API key in `tailor/MyCv.Tailor.Api/local.settings.json` (gitignored — never commit it):
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "GEMINI_API_KEY": "YOUR_KEY_HERE",
    "GEMINI_MODEL": "gemini-2.5-flash",
    "ALLOWED_ORIGINS": "https://localhost:7064"
  }
}
```

2. Start the Function App:
```bash
cd tailor/MyCv.Tailor.Api
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

# Tests

```bash
dotnet test MyCv.sln
```

# CI/CD

Deployments are handled by GitHub Actions (`.github/workflows/`).

| Trigger | Workflow | Deploys |
|---------|----------|---------|
| Push / PR to `develop` | `azure-static-web-apps-salmon-sand-05c416103.yml` | Front-end → **Development** SWA, version `dev` |
| PR to `main` | `azure-static-web-apps-icy-grass-0f365a003.yml` | Front-end → PR preview environment of the production SWA |
| Tag `v*` (e.g. `v1.2.0`) | `azure-static-web-apps-icy-grass-0f365a003.yml` | Front-end → **Production** SWA, version = tag name |
| Push to `main` touching `tailor/MyCv.Tailor.Api/**` | `feature-ai-tailor-search_mycv-api.yml` | Function App `mycv-api` |
| Push / PR to `main` or `develop` | `dotnet-main-dev.yml` → `dotnet.yml` | Build + tests only |

The front-end is built with `dotnet publish` in the workflow and uploaded pre-built (`skip_app_build: true`).
Build info (`Version`, `BuildDate`) is injected at build time via `sed` into `BuildInfo.cs`, and `<lastmod>` into `sitemap.xml`.

## Release

Git flow (details in [CLAUDE.md](CLAUDE.md#git-flow)): `feature/*` / `fix/*` branches → PR to `develop` → release.

1. Create `release/vX.Y.Z` from `develop` and open a PR to `main` (check the PR preview environment).
2. Merge the PR (this also deploys the Function App if `tailor/MyCv.Tailor.Api/**` changed).
3. Tag the merge commit on `main` and push: `git tag vX.Y.Z && git push origin vX.Y.Z`.
4. The production workflow deploys the front-end; the footer shows the tag and build date.
