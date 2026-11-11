# Roadmap

Prioritized improvements and known bugs for the live site (`ui/MyCv.UI.Wasm` + `tailor/MyCv.Tailor.Api`).
Tick items as they ship. Last review: 2026-11-10.

## Phase 0 — Documentation ✅

- [x] `CLAUDE.md`: repo map, commands, conventions, deploy flow, gotchas
- [x] README: wrong paths, model name, CI/CD table and "FakeTailorService in DEBUG" claim fixed; repo layout + release sections
- [x] `docs/ROADMAP.md`, `docs/CONTENT.md`, `docs/TAILOR.md`

## Phase 1 — Bugs & urgent

- [x] **.NET 8 → .NET 10 LTS** (WASM, Function, tests, workflows); MudBlazor 8.6 → 8.15; Functions worker packages updated
  - [ ] ⚠️ **Before merging to `main`**: switch the Function App stack to **.NET 10 isolated** in the Azure Portal
  - [ ] Verify the first SWA deploy on `develop` (front-end is now built with `dotnet publish` + `skip_app_build`)
- [x] Tailor API hardening: CORS restricted (`ALLOWED_ORIGINS`), input ≤ 4000 chars, per-IP rate limit (429), API key in header
- [x] 503 branch never matched (case-sensitive message check) → `GeminiUnavailableException` for Gemini 503/429
- [x] Error responses returned **200**: `WriteAsJsonAsync` resets the status code → errors written with `WriteStringAsync`
- [x] Tailor prompt out of sync with the site (AI-200 missing, Python/OpenTelemetry missing, Xcella treated as current job,
      vague past roles) → profile rewritten + prompt-injection rule
- [x] `Dispose()` never called: no component had `@implements IDisposable` (TailorSearch, Terminal, ThemeProvider, Education, Skill)
- [x] ScrollSpy re-created on every render through `eval` → single instance, `refreshScrollSpy()`
- [x] Tailor UI failed silently (deserialization errors, timeouts) → explicit messages, 45 s timeout, rate-limit message
- [x] `error.js` wiped the page on *any* JS error (extensions, CDN) → only during startup; `#blazor-error-ui` added
- [x] Language mismatch on first visit (loader followed the browser, app always French) → browser language by default, `<html lang>` set
- [x] No `staticwebapp.config.json` → SPA fallback, security headers, CSP (Report-Only)
- [ ] Switch the CSP from `Content-Security-Policy-Report-Only` to enforced once the browser console shows no violations on dev
- [ ] Set a budget / quota on the Gemini API key (the in-memory rate limiter is per instance, best-effort)
- [ ] Single source of truth for the profile (generate the prompt profile from the site data) so they can't drift apart again

## Phase 2 — SEO & sharing

- [ ] Prerender the home page at build time (WASM serves an almost empty HTML page to crawlers and link previews)
- [ ] Replace the hidden `display:none` `<h1>` in `index.html` with a real `<noscript>` / fallback summary
- [ ] `og:image` (1200×630), `twitter:card`, `<link rel="canonical">`, `og:locale` + alternate
- [ ] JSON-LD `Person` structured data (name, jobTitle, address, sameAs LinkedIn/GitHub)
- [ ] Portrait `Alt="Portrait"` → "Valérian Verona"

## Phase 3 — Performance

- [ ] `BlazorWebAssemblyLoadAllGlobalizationData=true` ships ICU data for every culture → `HybridGlobalization` or sharded ICU for fr/en/ja
- [ ] Drop Bootstrap (CSS + JS loaded on top of MudBlazor only for ScrollSpy and some utilities that overlap MudBlazor's)
- [ ] Pin + SRI the `flag-icons` CDN stylesheet (unversioned), or inline the 3 flags
- [ ] Long-term caching for fingerprinted `_framework/*` files (.NET 10 fingerprints them); check Brotli size
- [ ] Lazy-load the `WorldMap` (860-line inline SVG) and the AI video
- [ ] Roboto: `preconnect` + only the weights actually used

## Phase 4 — UX & accessibility

- [ ] Theme: respect `prefers-color-scheme` and save the choice (dark is currently forced on first render)
- [ ] Language menu: show the active language
- [ ] `AppBar.razor`: nav markup repeated 3× (mobile/desktop/monitor) with inconsistent labels (`CV` vs `DownloadCV`) → one component
- [ ] Mail icon uses the Gmail brand logo for a custom-domain address → generic email icon
- [ ] Tailor: example prompts / chips
- [ ] Lighthouse + axe pass: contrast of the light-mode tailor box, keyboard access to the marquee, `prefers-reduced-motion`

## Phase 5 — Repo hygiene

- [ ] Remove or move to `/legacy`: `ui/MyCv.UI`, `rating/*`, `common/*`, `docker-compose*`, `launchSettings.json`, `azurepipelines-coverage.yml`
- [x] Delete the dead `deploy.yml` / `deploy-dev.yml` / `deploy-prod.yml` (old Web Apps no longer exist) and the matching `deploy` job of `ai-codegen.yml`
- [ ] Remove the `CanNotDelete` lock on the `MyCV` resource group, or scope it to the Function/SWA resources only: it blocks the SWA "close" job from deleting PR preview environments (Free tier max 3), so PR checks fail
- [ ] Rename the remaining workflows to readable names (CI, SWA deploy dev/prod, Function deploy)
- [ ] Delete `VisitorService` + `visitoridstorage.js` (registered, never used, logs the ID as an error)
- [ ] Delete or gate `Playground.razor` (renders an empty page in production)
- [ ] Gemini model name as a configuration setting
- [ ] bUnit tests for `TailorSearch` states; function-level tests for CORS / 400 / 429
