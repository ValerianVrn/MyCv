# CLAUDE.md

Personal CV / portfolio website of Valérian Verona — https://www.valerian-verona.fr

## Git workflow
- Create a feature/<short-name> or fix/<short-name> branch from develop for each task.
- Never commit directly to main or develop.
- Make small commits with clear messages.
- Run the tests before committing.
- After finishing a feature or fix, commit it, push the branch, open a pull request into develop, and tell me it's ready to review.
- Before finishing, stop every server, or browser process you started.
- Merge only into develop, and only after I approve. Every merge into develop publishes the dev environment.
- Merge develop into main only when I ask for a release, through a release/<version> branch.
- Deploy production only from main, at a release.
- Never force push, rewrite shared history, or delete branches without asking.

## What is live

| Path | What | Deployed to |
|------|------|-------------|
| `ui/MyCv.UI.Wasm` | Blazor WebAssembly SPA (MudBlazor), FR/EN | Azure Static Web Apps |
| `tailor/MyCv.Tailor.Api` | Isolated Azure Function `POST /api/tailor` → Google Gemini | Azure Function App `mycv-api` |
| `tailor/MyCv.Tailor.Api.Tests` | MSTest + Moq tests for the Function | CI only |

**Legacy, not deployed:** `ui/MyCv.UI` (old Blazor Server app), `rating/*` (CQRS/ES rating microservices),
`common/*`, `docker-compose*`, `launchSettings.json`, `azurepipelines-coverage.yml`.
Do not modify them unless asked; don't copy patterns from them into the WASM app without checking.

## Commands

```bash
dotnet build MyCv.sln
dotnet test MyCv.sln

# Front-end (set TailorApi:UseFake=true in wwwroot/appsettings.Development.json to avoid calling the API)
cd ui/MyCv.UI.Wasm && dotnet watch run

# Function (needs Azure Functions Core Tools v4 + tailor/MyCv.Tailor.Api/local.settings.json)
cd tailor/MyCv.Tailor.Api && func start
```

## Conventions (WASM app)

- **Single page.** `Pages/Home.razor` composes the sections: `Hero`, `Skill`, `Career`, `Education`, `Ai`, `Life`.
  Each section has an `id` used as anchor by the nav items in `Shared/AppBar/AppBar.razor`.
- **Localization.** All visitor-facing text goes through `IStringLocalizer` and `.resx` files in
  `Resources/Components/{Layout,Pages}/` — always update **both** `X.resx` (English, default) and `X.fr.resx`.
  Culture is stored in `localStorage` (`wwwroot/scripts/culture.js`); changing language reloads the page.
- **Theme.** Palettes live in `Services/ThemeService.cs`. Components that subscribe to
  `ThemeService.OnThemeChanged` must declare `@implements IDisposable` and unsubscribe in `Dispose()`.
- **JS interop.** Plain scripts in `wwwroot/scripts/*.js`, referenced from `wwwroot/index.html`.
- **Styles.** Component-specific CSS goes in `*.razor.css` (scoped) or a `<style>` block in the component;
  global CSS in `wwwroot/css/`.
- **Formatting.** Follow `.editorconfig` (4 spaces; 2 for XML/csproj/resx).

## Content sync rule

The AI Tailor prompt (`tailor/MyCv.Tailor.Api/Prompt/TailorPrompt.cs`) contains a **copy** of the profile.
Whenever career, skills, certifications or availability change on the site, also update:
1. `tailor/MyCv.Tailor.Api/Prompt/TailorPrompt.cs`
2. `ui/MyCv.UI.Wasm/wwwroot/files/CV.pdf`
3. The terminal lines in `ui/MyCv.UI.Wasm/Shared/Terminal.razor` if relevant

See `docs/CONTENT.md` for the step-by-step checklist.

## Git flow

Long-lived branches: `main` (production) and `develop` (integration). **Never commit directly to either.**

```
develop ──┬── feature/<topic> ──PR──▶ develop ──▶ release/vX.Y.Z ──PR──▶ main ──▶ tag vX.Y.Z
          └── fix/<topic> ─────PR──┘
```

1. **Start work** from an up-to-date `develop`:
   `git fetch origin && git switch -c feature/<topic> origin/develop`
   - `feature/<topic>` for new things, `fix/<topic>` for bug fixes; kebab-case, sub-paths allowed (`feature/ui/home`).
   - **Never reuse a branch that was already merged** — start a new one.
2. **Commit** with conventional types, lower case: `feat:`, `fix:`, `perf:`, `build:`, `ci:`, `test:`, `refactor:`, `doc:`
   (this repo writes `doc`, not `docs`). One topic per commit; each commit should build.
3. **PR to `develop`** (`gh pr create --base develop`), merged with a **merge commit** (no squash/rebase).
   CI (`.NET` build + tests) runs on the PR; merging deploys the **dev** SWA (`salmon-sand-05c416103`).
4. **Release**: from `develop`, create `release/vX.Y.Z` (semver; `git tag` lists previous versions) → PR to `main`
   (this deploys a PR preview environment on the prod SWA) → merge.
5. **Tag** the merge commit on `main`: `git switch main && git pull && git tag vX.Y.Z && git push origin vX.Y.Z`
   → deploys **prod** SWA (`icy-grass-0f365a003`, www.valerian-verona.fr); the footer shows the tag.
   Release branches are **not** merged back into `develop`.

Other deploy triggers / notes:
- A push to `main` touching `tailor/MyCv.Tailor.Api/**` deploys the Function App — i.e. when the release PR is merged,
  before the tag. Infra changes the Function needs (e.g. runtime version) must be done in Azure first.
- CI rewrites `BuildInfo.cs` (Version, BuildDate) and `sitemap.xml` `<lastmod>` with `sed` — don't rely on hand edits there.
- Claude: only create branches/commits/PRs when asked; never push to `main`/`develop`, create `release/*` branches,
  or push tags without explicit instruction. Releases are the owner's call.

## Secrets

`GEMINI_API_KEY` lives only in the Function App settings and in `tailor/MyCv.Tailor.Api/local.settings.json`
(gitignored). Never commit it or put it in the WASM app — everything under `ui/MyCv.UI.Wasm/wwwroot` is public.
`ALLOWED_ORIGINS` (optional, comma-separated) controls the CORS origins echoed by the Function.

## Gotchas

- In the isolated Function, `HttpResponseData.WriteAsJsonAsync` **resets the status code to 200** — for error
  responses write the JSON with `WriteStringAsync` (see `TailorFunction.CreateErrorAsync`).
- `TailorFunction.MaxInputLength` and the UI's `TailorService.MaxInputLength` must stay equal.
- Tailor tests share the process-wide `GEMINI_API_KEY` env var → keep `[DoNotParallelize]` on `GeminiClientTest`.
- `wwwroot/staticwebapp.config.json` holds SPA fallback + security headers. The CSP is still **Report-Only**:
  if you add an external script/style/font/API origin, add it there too.

## Docs

- `README.md` — overview, hosting, local setup
- `docs/ROADMAP.md` — prioritized improvements / known bugs
- `docs/CONTENT.md` — how to update CV content
- `docs/TAILOR.md` — AI Tailor contract, errors, local testing
