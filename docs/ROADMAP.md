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

- [x] Hidden `display:none` `<h1>` replaced by a real `<noscript>` summary (French)
- [x] French title + description, canonical, Open Graph (`og:image` 1200×630, `og:locale` fr_FR + en_US), Twitter card
- [x] JSON-LD `Person` (jobTitle, address, alumniOf, knowsAbout, sameAs LinkedIn/GitHub)
- [x] Portrait alt text; `<PageTitle>` aligned with the static title
- [ ] After the release: validate with Google Rich Results Test + LinkedIn Post Inspector, submit the sitemap in Search Console
- [ ] Prerender the home page at build time. Deferred: `Program.cs` calls JS interop (`blazorCulture`) before `RunAsync`,
      which build-time prerenderers can't run. Google renders the app itself, and link previews now use the static tags

## Phase 3 — Performance

Measured on a Release publish (2026-11-12): `_framework` Brotli **3.38 MB → 2.86 MB**; uncompressed first load in headless
Chrome **13.8 MB → 10.4 MB**.

- [x] World map: 1.2 MB of SVG paths were compiled into the app assembly (405 KB → 63 KB Brotli). Now
      `wwwroot/images/world-map.svg`, fetched only when the Life section approaches (`loadWorldMap` in `scripts/map.js`)
- [x] ICU: all locales → `icudt_EFIGS.dat` shard (322 KB → 140 KB Brotli). Checked with fr-FR, en-US and ja-JP browsers
- [x] AI video: no autoplay/preload, started by `ai-carousel.js` only when visible
- [x] `flag-icons` pinned to 7.6.0 with SRI
- [x] Roboto: `preconnect` + only weights 300–700 and 400 italic
- [ ] Drop Bootstrap (CSS + JS loaded on top of MudBlazor only for ScrollSpy and a few utilities: `me-2`, `end-0`, `top-0`,
      `d-md-*`…). Needs a visual before/after check, because Bootstrap and MudBlazor breakpoints differ (768 vs 960 px)
- [ ] Long-term caching for fingerprinted `_framework/*` files (.NET 10 fingerprints them)
- [ ] Reduce `dotnet.native.wasm` (953 KB Brotli): check `InvariantTimezone`, trimming options

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
- [x] Stale PR preview environments deleted (2026-11-11)
- [x] Dev site allowed in the Function's platform CORS (the Tailor works on dev); platform CORS documented as the source of truth
- [ ] The `MyCv-delete` CanNotDelete lock is at **resource-group** scope: it blocks the SWA "close" job, so PR previews will pile up
      again (Free tier max 3). Move the lock to the individual resources instead, or delete previews by hand after each PR
- [ ] Dev Function App (`mycv-api-dev`, Flex Consumption) deployed from `develop`, so API changes can be tested on dev before a
      release and dev traffic doesn't share the prod Gemini quota
- [ ] Rename the remaining workflows to readable names (CI, SWA deploy dev/prod, Function deploy)
- [ ] Delete `VisitorService` + `visitoridstorage.js` (registered, never used, logs the ID as an error)
- [ ] Delete or gate `Playground.razor` (renders an empty page in production)
- [ ] Gemini model name as a configuration setting
- [ ] bUnit tests for `TailorSearch` states; function-level tests for CORS / 400 / 429
