# Updating the CV content

All content lives in the Blazor WASM app (`ui/MyCv.UI.Wasm`). The data is written in C# in the `.razor` files,
and the visible text lives in `.resx` files: `X.resx` is English (the default) and `X.fr.resx` is French.
**Always update both.**

## Add or edit a job

1. `Pages/Career.razor` → `OnInitialized()`: add a `new ()` entry at the top of `_experiences` (most recent first).
   - `CompanyLogo`: add the logo to `wwwroot/images/companies/` (`.webp`, square, small).
   - `StartDate` / `EndDate`: the duration is computed automatically (`ExperienceCardContent.razor`).
   - `Skills`: free-text chips.
   - `IsInternship = true` for internships.
2. Add the text keys (`SectionExperience<Company>JobTitle`, `…Place`, `…ProjectTitle`, `…Task1..n`) to
   `Resources/Components/Pages/Career.resx` **and** `Career.fr.resx`.

## Add a skill / technology

- `Pages/Skill.razor`:
  - `_coreSkills`: the highlighted chips at the top
  - `MainSkills`: the cards, each with a list of technologies (`new("Name", "images/technology/x.webp")`)
- Icons go in `wwwroot/images/technology/`.

## Add a certification

- `Pages/Education.razor` → `_microsoftCertifications` or `_languageCertifications`.
- Badge images go in `wwwroot/images/certifications/`. For Microsoft certs, use the Microsoft Learn credential share URL.
- If it's an Associate (or higher) certification, the portrait badge in `Pages/Hero.razor` may need updating.

## Update the downloadable CV

Replace `wwwroot/files/CV.pdf` (keep the same file name: the AppBar buttons link to it).

## AI section: project cards

`Pages/Ai.razor` → "Built with AI". Texts are in `Resources/Components/Pages/Ai.resx` / `Ai.fr.resx`
(`QuadlinkDescription`, `QuadlinkProof`, `Instagram`…).

- **Numbers** in `QuadlinkProof` (days, pull requests, commits) come from the Quadlink GitHub history. Keep the
  "part-time" wording, and update them if you mention a newer version.
- **Screenshots** `wwwroot/images/projects/quadlink-en.webp` / `quadlink-fr.webp` (390×670): the quadlink.fr home
  screen at phone size, one per language. The card picks the visitor's language.
- **Stack chips**: `QuadlinkStack` in the `@code` block.
- **New project**: copy the Quadlink `MudPaper` card. Keep the small tag that says *which* AI (Claude Code for
  AI-assisted development, generative AI for automated content).
- Also update the Tailor prompt ("Side projects") and the `<noscript>` "Projets" list in `index.html`.

## Mark a country as visited

The world map is a static file: `wwwroot/images/world-map.svg`. Each country is a `<path id="FR" title="France" …>`;
add `class="visited"` to it (the tooltip shows its `title`). Styles live in `Pages/WorldMap.razor`.

## Change the hero terminal

`Shared/Terminal.razor` → default `TerminalCommands` (whoami / stack / status).

## Tailor prompt sync checklist

The AI Tailor does not read the site. It uses a copy of the profile in
`tailor/MyCv.Tailor.Api/Prompt/TailorPrompt.cs`. After any change above, check:

- [ ] **Identity / availability**: matches the terminal `status` line
- [ ] **Core skills**: same as `_coreSkills` in `Skill.razor`
- [ ] **Technologies**: same as the `MainSkills` technologies
- [ ] **Experience**: companies, titles, dates, key projects from `Career.razor` / `Career.resx`
- [ ] **Education & certifications**: same as `Education.razor`
- [ ] `CV.pdf` tells the same story
- [ ] **SEO copy** in `wwwroot/index.html` (French): title, description, OG/Twitter tags, JSON-LD `Person`
      (jobTitle, knowsAbout, alumniOf) and the `<noscript>` summary (experience list)
- [ ] **Share image** `wwwroot/images/og-image.jpg` (1200×630) if the title or certifications change.
      Check the preview with LinkedIn Post Inspector (https://www.linkedin.com/post-inspector/), which also refreshes LinkedIn's cache

The Function redeploys automatically when the change is merged to `main` (the workflow triggers on `tailor/MyCv.Tailor.Api/**`).

## Before releasing

- Run the site locally in **both** languages (language menu → it reloads the page).
- Check mobile width (the hero, skills and career sections have separate mobile layouts).
- Ship as described in the README ("Release").
