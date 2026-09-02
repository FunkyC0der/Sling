# AGENTS.md

This file is already in context. Do not read it again.
Owner instructions in this message override every other rule until the owner says otherwise in this message.

AngryMeatBoy — 2D slingshot platformer. Unity **6000.4.3f1**, C# **9 max** (no collection expressions, no positional record structs). Code: `Assets/_Game/Scripts/`.

## Process

- Preflight: `git status --short` only. No install, build, compile, or Unity/`dotnet` unless this message asks.
- Search narrowly. Do not read unchanged files, secrets, lockfiles, generated output, `Library/`, `Temp/`, `graphify-out/`, `Assets/Screenshots/`, or `**/*.html`.
- Verify only affected targeted tests. After two identical infrastructure failures, diagnose — do not rerun.
- Independent obvious edits: do them. Cross-cutting design: ask first.
- Change docs only if this message asks. Do not follow Related / See also links if one file already answers.

## Conventions

- HMVC: never `new` a controller — `Execute<T>` / `ExecuteAndWaitResultAsync<T>` only; no controller→controller refs (POCO event brokers); View talks to Controller via `event` only; Model is POCO (no Unity objects); Config is tuner SO or `GameConfig` section, no runtime state.
- Controllers may use the Unity API when the feature needs it. Required views are ctor params; optional views via `IOptionalViewProvider` in ctor — store the view, not the provider.
- `CancellationToken` on every await. Subscribe in `OnStart`; cleanup via `AddDisposable`. Dispose Unity objects the controller created.
- Inspector fields: `public` + `_underscore` (never `[SerializeField] private`). Do not mass-convert existing private SerializeFields. `[SerializeReference]` for polymorphic types. Private constants: `_kKPascalCase`.
- No `FindObjectOfType` / singletons / static locators. No gameplay numbers outside configs. No comments unless WHY is non-obvious. New Input System only. 2D physics only (`Rigidbody2D`, `Physics2D`).
- Folder = DI scope: `Root/` ↔ `GameBootstrapper`; `Level/` ↔ `BuildLevelScopeController`; `Common/` only if used by ≥2 scopes; namespace mirrors path; one feature = one flat folder. New feature: Config → Model? → Controller → View? → register in matching scope → `Execute` from parent.

## Router (pick one; do not chain)

- UI (`.uxml` / `.uss` / UIDocument): `.agents/skills/ui-uitk/SKILL.md` — this project is UI Toolkit, not uGUI.
- Kanban / `.devtool/features/`: `.agents/skills/kanban-markdown/SKILL.md`
- Level foliage on Ground tops: `.agents/skills/place-ground-decorations/SKILL.md`
- Audio import / mixer memory: `.agents/skills/optimize-audio/SKILL.md`
- Pixel grid: `Documents/PIXEL_LAYOUT.md`
- Palette: `Documents/color-palette-sling-hero.md`
- Drive live Unity Editor / CLI: `.agents/skills/unity-cli/SKILL.md`

Default: do not open docs or skills. Do not walk their related links.
