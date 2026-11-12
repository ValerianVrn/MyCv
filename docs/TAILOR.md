# AI CV Tailor

A recruiter pastes a job title or description in the hero section. The front-end sends it to an
Azure Function, which asks Google Gemini to compare it with Valérian's profile and returns a structured verdict.

```
TailorSearch.razor ──▶ Services/TailorService.cs ──POST──▶ TailorFunction.cs ──▶ GeminiClient.cs ──▶ Gemini
   (ui/MyCv.UI.Wasm)                                      (tailor/MyCv.Tailor.Api)
```

## Contract

**Request** `POST /api/tailor`

```json
{ "input": "Senior .NET developer, Azure, microservices" }
```

**Response** `200 OK`

```json
{
  "case": 3,
  "stars": 5,
  "matchLabel": "Strong match",
  "humor": "…",
  "whyMatch": ["Azure", "C#/.NET"],
  "skillBridges": [{ "asked": "AWS", "have": "Azure" }],
  "bonusSkills": ["Event Sourcing"],
  "pitch": "…",
  "contactCopy": "…"
}
```

| case | Meaning | stars |
|------|---------|-------|
| 1 | Not relevant | 0 |
| 2 | Partial match (transferable skills, skill bridges) | 1–3 |
| 3 | Strong match | 4–5 |

The Gemini output is validated by `Validators/TailorResultValidator.cs`. An output that breaks these rules
(for example wrong stars for the case, or empty `whyMatch` for case 2/3) is rejected with a 500.

## Errors & limits

| Status | Body | When | UI message |
|--------|------|------|------------|
| 400 | `{"error":"empty_input"}` / `"invalid_json"` / `"input_too_long"` | Bad request; input > 4000 chars | "unavailable" (the UI field already caps at 4000) |
| 429 | `{"error":"rate_limited"}` | More than 10 requests / 10 min from the same IP (per Function instance) | `TailorRateLimited` |
| 503 | `{"error":"gemini_unavailable"}` | Gemini answered 503 or 429 after the Polly retries (2s, 4s, 8s) | `TailorUnavailable` |
| 500 | text | Anything else (invalid Gemini JSON, validation failure, missing key) | `TailorUnavailable` |

The front-end also gives up after **45 s** (`TailorSearch.razor`, `RequestTimeout`).

The limits live in `TailorFunction.MaxInputLength`, which must match `TailorService.MaxInputLength` in the UI,
and in `RateLimiter.DefaultPermitLimit` / `DefaultWindow`.
The rate limiter is in-memory and works per instance. It stops bursts from one visitor, but it is not a quota.
For a hard cap, also set a budget / quota on the Gemini API key in Google AI Studio.

## Security

- The model comes from the optional `GEMINI_MODEL` app setting (default `gemini-2.5-flash`), so it can be changed
  in the Function App settings without redeploying.
- The API key is read from the `GEMINI_API_KEY` app setting and sent in the `x-goog-api-key` header,
  not in the query string.
- CORS: **platform CORS on the Function App is the source of truth** (prod site, dev site, `localhost:7064`, Azure Portal;
  see the README). The code-level CORS (`ALLOWED_ORIGINS`, default `https://www.valerian-verona.fr`) is only a fallback
  for when platform CORS is off, for example under `func start`. PR previews are not allowed origins.
- Prompt injection: the system prompt tells the model to treat the recruiter text as data only.
  Output is rendered as text by Blazor, never as HTML.
- Privacy: the UI tells visitors that their text is sent to Google Gemini. The function does not store the input.
  It logs the raw Gemini response at `Debug` level only, which is off by default.

## The profile in the prompt

`Prompt/TailorPrompt.cs` contains a **hand-written copy** of the profile. Keep it in sync with the site;
see [CONTENT.md](CONTENT.md#tailor-prompt-sync-checklist).

## Local testing

1. `tailor/MyCv.Tailor.Api/local.settings.json` (gitignored):
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
2. `cd tailor/MyCv.Tailor.Api && func start`
3. In `ui/MyCv.UI.Wasm/wwwroot/appsettings.Development.json` set
   `"TailorApi": { "Url": "http://localhost:7071/api/tailor", "UseFake": false }`.
4. Quick checks without the UI:
   ```bash
   curl -i -X POST http://localhost:7071/api/tailor -H "Content-Type: application/json" -d '{"input":"Senior .NET Azure"}'
   curl -i -X POST http://localhost:7071/api/tailor -H "Content-Type: application/json" -d '{"input":""}'   # 400
   ```

To work on the UI without calling Gemini, set `"UseFake": true`. `Services/FakeTailorService.cs` then returns canned results.

## Tests

`tailor/MyCv.Tailor.Api.Tests` covers Gemini response parsing and validation, the 503/429 mapping,
the API key header, the rate limiter and client IP parsing. Run them with `dotnet test tailor/MyCv.Tailor.Api.Tests`.
