# План: Інтеграція HMVC (Playtika Controllers Tree) в AngryMeatBoy

## Context

**Гра:** Sling Platformer — 2D platformer, де гравець керує персонажем через Drag+Aim+Release (як Angry Birds), без mid-air control. Single-screen рівні з небезпеками (spikes, saws, lasers, saw cannons), різними поверхнями (sticky/bouncy walls, crumbling/moving platforms, gravity fans), миттєвий рестарт (<0.5с). MVP: 10 рівнів, головний персонаж, sling launch, finish, instant restart, spikes + saw + moving saw. У майбутньому — boss з фазами, premium, mobile/PC/console.

**Поточний стан:** Один сценарій-механіка [Assets/Scripts/DragAndLaunch.cs](Assets/Scripts/DragAndLaunch.cs), єдина сцена `Assets/Scenes/SampleScene.unity`. Жодної архітектури.

**Що вирішено:** скинути всі попередні рішення, починати з нуля на HMVC (Hierarchical MVC, "Controllers Tree") від Playtika. Пакет уже встановлений — `com.playtika.controllers-tree@v1.2.0`, разом із залежністю UniTask. Усі попередні архітектурні документи (`ArchNotes/*`) **ігноруються** як застарілі.

**Чому HMVC:** code-first, єдина точка входу (RootController), малі класи з чітким SRP, RAII-управління ресурсами через життєвий цикл контролерів, явне керування CancellationToken (критично для instant-restart механіки де треба обірвати анімації/таймери), tree viewer для debug. Підходить навіть для соло-розробки — boilerplate компенсується чіткістю та тестованістю.

**Прийняті рішення:**
- DI: **власна `ControllerFactory`** на словнику `Type → Func<IController>` без зовнішнього контейнера.
- Структура: **за фічею, плоскі папки** (`Scripts/Player/`, `Scripts/Hazards/` тощо, без `Scripts/Features/`).
- Views: **без Sisus Init(args)** — звичайний `Bind(...)` метод що викликається контролером.

---

## Архітектурна модель

### Три шари

| Шар | Тип | Призначення |
|-----|-----|-------------|
| **Controller** | `class : ControllerBase` / `ControllerWithResultBase` | Бізнес-логіка, оркестрація, виклики дочірніх контролерів. Pure C#. |
| **Model** | POCO `class` (можливо з `event`s) | Дані стану + події про зміни. Pure C#, без посилань на UnityEngine.* (окрім `Vector2/Quaternion/Color`). |
| **View** | `MonoBehaviour` | Тонкий міст до Unity API — рендеринг, інпут, фізика. **Жодної бізнес-логіки**, лише `Bind(...)` + події назовні + методи `Show/Hide/SetValue`. |

### Configs (ScriptableObject)

SO використовується **тільки** для immutable дизайнерських тюнерів (швидкості, сили, тайминги, drops). SO ніколи не тримає runtime-стан і не має методів логіки. Приклади: `PlayerConfig`, `LevelConfig`, `HazardConfig`.

### Дерево контролерів (target shape)

```
GameRoot (LaunchTree один раз з MonoBehaviour-bootstrap)
├── BootstrapController (ControllerWithResult)            // load configs, services
├── GameLoopController (ControllerBase)                   // живе все життя додатка
│   ├── MainMenuController (ControllerWithResult<LevelId>) // показує меню, чекає вибір
│   └── LevelSessionController (ControllerWithResult<SessionResult>)
│       ├── LoadLevelController (ControllerWithResult)    // завантаження рівня
│       ├── LevelGameplayController (ControllerWithResult<GameplayOutcome>)
│       │   ├── PlayerController (ControllerBase)         // обробка drag+launch, керує PlayerView
│       │   ├── HazardsController (ControllerBase)        // підписка на hazards' OnHit
│       │   ├── SurfacesController (ControllerBase)       // sticky/bouncy/crumble логіка
│       │   ├── FinishController (ControllerBase)         // тригер фінішу
│       │   ├── TimerController (ControllerBase)          // speedrun таймер
│       │   └── HudController (ControllerBase)            // показ таймера, restart-кнопка
│       └── LevelResultController (ControllerWithResult<NextAction>) // Won/Lost screen
```

**Гранди:**
- Restart рівня = `Complete(Restart)` поточного `LevelGameplayController` → cancellation token обриває все, дерево згортається, `LevelSessionController` запускає той самий рівень знову.
- Перехід між рівнями = `LevelSessionController` повертає `SessionResult`, `GameLoopController` запускає наступний `LevelSessionController`.
- Все cancellation-aware за замовчуванням.

### Комунікація між контролерами

Прямого зв'язку між контролерами **немає**. Контролер не знає про інші контролери — тільки про свого батька (через `Execute<T>`) і власні дочірні. Якщо двом sibling-контролерам потрібно реагувати на одну й ту ж подію — використовується **POCO-модель-посередник із самими лише event-ами** (без логіки, без runtime-стану крім тригер-полів).

**Приклад:** `HazardsController` детектує удар → виставляє `LevelEvents.RaisePlayerDied()` → `LevelGameplayController` слухає `LevelEvents.OnPlayerDied` і викликає `Complete(Death)`. Жоден з них не має посилання один на одного.

Правило: **Models = data + events; ніяких прямих викликів controller→controller**.

---

## Кроки впровадження

### Крок 1: Налаштувати ControllerFactory та bootstrap

**Файл:** `Assets/Scripts/Core/ControllerFactory.cs`

Власна реалізація `IControllerFactory` без зовнішнього DI:

```csharp
public sealed class ControllerFactory : IControllerFactory
{
    private readonly Dictionary<Type, Func<IController>> _builders = new();

    public ControllerFactory Register<T>(Func<T> builder) where T : class, IController
    {
        _builders[typeof(T)] = builder;
        return this;
    }

    public IController Create<T>() where T : class, IController
    {
        if (!_builders.TryGetValue(typeof(T), out var builder))
            throw new InvalidOperationException($"Controller {typeof(T).Name} not registered");
        return builder();
    }
}
```

**Файл:** `Assets/Scripts/Core/GameBootstrapper.cs` — єдиний `MonoBehaviour` на сцені `Bootstrap.unity`. Створює `ControllerFactory`, реєструє всі сервіси/моделі/контролери у замиканнях (`() => new PlayerController(factory, playerModel, playerConfig, inputService)`), створює `GameRootController`, викликає `LaunchTree(this.GetCancellationTokenOnDestroy())`.

**Файл:** `Assets/Scripts/Core/GameRootController.cs` — `RootController`, в `OnStart()` запускає `BootstrapController` через `ExecuteAndWaitResultAsync`, потім `GameLoopController`.

### Крок 2: Розгорнути сцени

- `Assets/Scenes/Bootstrap.unity` — нова сцена з `GameBootstrapper` і нічим іншим. Додати першою у Build Settings.
- `Assets/Scenes/MainMenu.unity` — нова, з `MainMenuView` (UI Canvas).
- `Assets/Scenes/Levels/Level_01.unity` ... `Level_10.unity` — рівні. Поточна `SampleScene.unity` стає `Level_01.unity` (перейменувати).
- Перехід між сценами через `SceneManager.LoadSceneAsync` всередині відповідних контролерів (`MainMenuController` ховає себе, `LoadLevelController` вантажить рівень).

### Крок 3: Створити моделі (POCO)

- `Assets/Scripts/Player/PlayerModel.cs` — `Vector2 Position`, `Vector2 Velocity`, `bool IsLaunched`, `int LaunchCount`; події `event Action OnLaunched`, `event Action OnDied`.
- `Assets/Scripts/Level/LevelModel.cs` — `int LevelId`, `bool IsFinished`, `float ElapsedSeconds`; події `event Action OnFinished`, `event Action OnRestartRequested`.
- `Assets/Scripts/Game/GameProgressModel.cs` — `int CurrentLevel`, `int TotalDeaths`.

### Крок 4: Створити Views (MonoBehaviour)

- `Assets/Scripts/Player/PlayerView.cs` — переписаний `DragAndLaunch.cs`. Має `Rigidbody2D`, читає `InputActionReference`, генерує події `event Action<Vector2> OnLaunchRequested(force)` для `PlayerController`. Метод `Bind(PlayerModel model, PlayerConfig config)` викликається контролером в `OnStart`.
- `Assets/Scripts/Hazards/HazardView.cs` — базовий клас із `Collider2D`-trigger детекцією. Конкретики: `SpikeView`, `SawView`, `MovingSawView`, `LaserView`, `SawCannonView`. Кожен кидає `event Action OnPlayerHit`.
- `Assets/Scripts/Surfaces/SurfaceView.cs` — базовий + `StickyWallView`, `BouncyWallView`, `CrumblingPlatformView`, `MovingPlatformView`, `GravityFanView`.
- `Assets/Scripts/Level/FinishView.cs` — trigger, `event Action OnPlayerReachedFinish`.
- `Assets/Scripts/UI/HudView.cs`, `MainMenuView.cs`, `LevelResultView.cs` — UI Canvas.

### Крок 5: Створити Configs (ScriptableObject)

- `Assets/Scripts/Player/PlayerConfig.cs` (`[CreateAssetMenu]`) — `MaxDragDistance`, `LaunchForceMultiplier`, `WallSlideSpeed`.
- `Assets/Scripts/Hazards/HazardConfig.cs` — параметри per-тип hazard.
- `Assets/Scripts/Level/LevelDatabase.cs` — список `LevelConfig` (sceneName, displayName, parTime).
- Створити `.asset` файли в `Assets/Configs/`.

### Крок 6: Створити контролери MVP

Мінімум для гри в один рівень:

| Файл | Тип | Args/Result | Призначення |
|------|-----|-------------|-------------|
| `Assets/Scripts/Core/GameRootController.cs` | `RootController` | — | Кореневий, оркеструє bootstrap+game loop |
| `Assets/Scripts/Core/BootstrapController.cs` | `ControllerWithResultBase` | — | Завантажує конфіги, готує сервіси |
| `Assets/Scripts/Core/GameLoopController.cs` | `ControllerBase` | — | Менеджмент циклу меню/гра |
| `Assets/Scripts/UI/MainMenuController.cs` | `ControllerWithResultBase<int>` | result: levelId | Показує меню, чекає вибір рівня |
| `Assets/Scripts/Level/LevelSessionController.cs` | `ControllerWithResultBase<LevelSessionArgs, SessionResult>` | levelId → outcome | Орекструє один цикл рівня |
| `Assets/Scripts/Level/LoadLevelController.cs` | `ControllerWithResultBase<int>` | args: levelId | LoadSceneAsync, повертає посилання на views |
| `Assets/Scripts/Level/LevelGameplayController.cs` | `ControllerWithResultBase<GameplayOutcome>` | result: Win/Death/Quit | Активний геймплей |
| `Assets/Scripts/Player/PlayerController.cs` | `ControllerBase` | — | Слухає `PlayerView.OnLaunchRequested`, оновлює `PlayerModel` |
| `Assets/Scripts/Hazards/HazardsController.cs` | `ControllerBase` | — | Підписка на всі `HazardView.OnPlayerHit` → `LevelGameplayController` |
| `Assets/Scripts/Level/FinishController.cs` | `ControllerBase` | — | Підписка на `FinishView.OnPlayerReachedFinish` |
| `Assets/Scripts/UI/LevelResultController.cs` | `ControllerWithResultBase<NextAction>` | result: Restart/Next/Menu | Показує результат |

Crumbling/Moving/Bouncy/Sticky/Fan + Saw cannon + Laser + Boss — додаються інкрементно після MVP.

### Крок 7: Видалити застарілий код

- Видалити `Assets/Scripts/Test.cs` (placeholder).
- Перенести логіку з `Assets/Scripts/DragAndLaunch.cs` у `PlayerView` + `PlayerController`. Старий файл видалити.
- Видалити папку `ArchNotes/` як застарілу (з підтвердженням).

### Крок 8: Оновити CLAUDE.md

Замінити секцію "Code Architecture" на новий гайдлайн (детально нижче в розділі **Гайдлайн**).

### Крок 9: Оновити memory-індекс

Замінити `~/.claude/projects/.../memory/project_architecture.md` посилання на цей `ARCHITECTURE.md`.

---

## Гайдлайн (для CLAUDE.md та для людської пам'яті)

### Правила

**П1. Будь-яка нова фіча — починається з SO-Config (game-design-driven).**
Спочатку дизайнер описує тюнабельні параметри у ScriptableObject (швидкість, сили, тайминги, дропи). Потім модель (POCO) — які дані стану і події потрібні. Потім контролер, який читає Config + керує моделлю + викликає View. View — останній шар, тонкий міст до Unity. Послідовність: **Config → Model → Controller → View**.

**П2. `ControllerBase` vs `ControllerWithResultBase`.**
- `ControllerBase` — для речей, що живуть весь час, поки живе батько (HUD, підписки на input, hazards listener).
- `ControllerWithResultBase` — для речей зі своїм фінішем: екран меню, рівень, діалог, асинхронне завантаження. **`Complete(result)` або `Fail(exception)` — обов'язкові.**

**П3. Models — POCO, без UnityEngine.GameObject-ієрархії.**
Дозволено: примітиви, `Vector2/3/4`, `Quaternion`, `Color`, посилання на SO-Configs. Заборонено: `MonoBehaviour`, `Transform`, `Rigidbody2D`, `Collider2D`, `GameObject`. Це робить моделі testable і не прив'язує до Unity-lifecycle.

**П4. Views — тонкі.**
View має: серіалізовані поля, `Bind(...)` метод, методи `Show/Hide/SetX`, події назовні. View **не звертається** до моделей напряму, не містить game logic, не викликає інші views.

**П5. Configs — лише дизайнерські тюнери.**
SO без runtime-стану. Якщо в SO є метод що міняє стан або `OnEnable`/`OnDisable` з логікою — це сигнал що це не Config, а сервіс (треба робити POCO).

**П6. Boilerplate args/result — окремі структи.**
`PlayerControllerArgs`, `LevelSessionResult` тощо. `EmptyControllerArg`/`EmptyControllerResult` коли не треба.

**П7. CancellationToken — пропагується в усі async-операції.**
Будь-який `await UniTask.Delay`, `await SceneManager.LoadSceneAsync(...).ToUniTask(...)`, мережевий запит — обов'язково з `cancellationToken`. Він приходить параметром у `OnFlowAsync`.

**П8. Підписки — у `OnStart`, відписка — у `OnStop`.**
Або через `AddDisposable(new DisposableToken(() => model.OnDied -= Handler))`. Це гарантує очистку навіть при exception.

**П9. View → Controller: лише через C# events.**
View не знає про модель. View кидає `event Action<Vector2> OnLaunchRequested`. Controller підписується в `OnStart`, обробляє, оновлює модель, викликає методи view.

**П10. Controller → Controller: пряма комунікація заборонена.**
Контролер знає тільки про свого батька (через `Execute<T>` / `ExecuteAndWaitResultAsync<T>`) і реєстр власних дочірніх. Не зберігай посилання на дочірній контролер, не шукай sibling-контролер.

Якщо двом контролерам треба реагувати на одну подію — створюється POCO-**модель-посередник** з самими лише `event`-ами (без логіки, без runtime-стану крім тригер-полів). Обидва контролери інжектять цю модель з фабрики (singleton-instance або scoped per level). Один — викликає `Raise...()`, інший — підписується на `event`.

Приклад: `LevelEvents { event Action OnPlayerDied; event Action OnFinishReached; void RaisePlayerDied(); void RaiseFinishReached(); }`. `HazardsController` викликає `Raise`, `LevelGameplayController` слухає `event`. Жодних прямих посилань.

### Anti-patterns (червоні прапорці)

- `Singleton`-MonoBehaviour, `static` service-locator, `FindObjectOfType` — заборонено. Усе через `ControllerFactory`.
- `public` методи на контролерах (окрім `RootController.LaunchTree`) — заборонено. Запуск тільки через `Execute<T>()`.
- Створення контролера через `new` поза фабрикою — заборонено.
- Пряме посилання controller→controller (зберігання іншого контролера в полі, виклик його методу) — заборонено. Тільки через POCO-event-broker.
- Логіка в `Awake/Start` MonoBehaviour поза `Bind(...)` — заборонено. View чекає на `Bind`.
- Зберігання runtime-стану в SO — заборонено.
- `MonoBehaviour` у POCO-моделях — заборонено.
- `GameObject.Find` / `FindObjectsByType` — заборонено. Посилання інжектяться через factory або серіалізовані view-поля.
- Hard-coded числа поза SO-Configs (швидкості, сили, тайминги) — заборонено. Винось у Config.
- Забутий `cancellationToken` в `await` — заборонено. Завжди передавай.
- `GetComponent` поза View-кодом — заборонено. View вже знає свої компоненти.

### Стартовий чек-лист для нової фічі (порядок суворий)

1. **Config (SO)** — які тюнабельні параметри потрібні дизайнеру? Створити ScriptableObject. Без runtime-стану.
2. **Model (POCO)** — який стан тримати? Які `event`-и кидати про зміни стану? Без посилань на UnityEngine.GameObject-ієрархію.
3. **Контролер** — це one-shot (controller-with-result) чи живе-постійно (controller-base)? Які Args/Result? Створити структу.
4. **Чи треба слухати інший контролер?** — якщо так, створити POCO-event-broker (`*Events` клас з самими event-ами). Не дозволено: пряме посилання controller→controller.
5. **View (MonoBehaviour)** — як це показати/обробити інпут? Які `event`-и view кидає назовні? `Bind(...)` метод приймає Model+Config від контролера.
6. **Реєстрація** — додати у `GameBootstrapper`: `factory.Register<TController>(() => new TController(factory, ...));`.
7. **Запуск** — з батьківського контролера через `Execute<T>()` або `ExecuteAndWaitResultAsync<T>()`.
8. **Cleanup** — усі підписки в `OnStart`, очистка через `AddDisposable(new DisposableToken(() => model.OnX -= Handler))`.
9. **Async** — усі `await` з `cancellationToken`.

### Коли переглянути архітектуру

- Якщо понад **3 рівні вкладеності** controller-tree для однієї фічі → виглядає як over-engineering.
- Якщо в одному контролері > **300 рядків** → ділимо на дочірні.
- Якщо View починає містити `if`/`switch` по бізнес-логіці → переносимо у Controller.
- Якщо Model має методи що звертаються до Unity API → виносимо в View або Service-POCO.

---

## Критичні файли (підсумок)

| Файл | Дія |
|------|-----|
| `Assets/Scripts/Core/ControllerFactory.cs` | Створити |
| `Assets/Scripts/Core/GameBootstrapper.cs` | Створити (MonoBehaviour) |
| `Assets/Scripts/Core/GameRootController.cs` | Створити |
| `Assets/Scripts/Core/BootstrapController.cs` | Створити |
| `Assets/Scripts/Core/GameLoopController.cs` | Створити |
| `Assets/Scripts/Player/PlayerController.cs` | Створити |
| `Assets/Scripts/Player/PlayerModel.cs` | Створити |
| `Assets/Scripts/Player/PlayerView.cs` | Створити (з логіки `DragAndLaunch.cs`) |
| `Assets/Scripts/Player/PlayerConfig.cs` | Створити |
| `Assets/Scripts/Level/*` | Створити (Session/Load/Gameplay/Result + Model + Views) |
| `Assets/Scripts/Hazards/*` | Створити (HazardsController + базовий View + Spike/Saw/MovingSaw для MVP) |
| `Assets/Scripts/UI/*` | Створити (MainMenu, Hud, LevelResult — controllers + views) |
| `Assets/Scripts/DragAndLaunch.cs` | **Видалити** після переносу |
| `Assets/Scripts/Test.cs` | **Видалити** |
| `Assets/Scenes/Bootstrap.unity` | Створити |
| `Assets/Scenes/MainMenu.unity` | Створити |
| `Assets/Scenes/SampleScene.unity` → `Levels/Level_01.unity` | Перейменувати |
| `Assets/Scenes/Levels/Level_02..10.unity` | Створити (можна як копії шаблону) |
| `Assets/Configs/PlayerConfig.asset` | Створити |
| `Assets/Configs/LevelDatabase.asset` | Створити |
| `CLAUDE.md` | Оновити секцію "Code Architecture" + додати "HMVC Guideline" |
| `ArchNotes/` | **Видалити** (з підтвердженням) |

---

## Не входить у scope цього плану

- **Not done:** unit-тести з `SubstituteControllerFactory` — додамо, коли буде що тестувати (після MVP).
- **Not done:** Boss-механіка — після того, як 10 рівнів MVP працюють.
- **Not done:** Surfaces (sticky/bouncy/crumble/moving/fan), Saw cannon, Laser — додаються інкрементно після першого рівня з spikes+saw.
- **Not done:** Audio, particles, juice — після того, як механіка стабільна.
- **Not done:** збереження прогресу, leaderboards, IAP — premium-ціль; після MVP.

---

## Верифікація після інтеграції

1. **Компіляція:** проект відкривається в Unity без помилок.
2. **Manual smoke test:**
   - Відкрити `Bootstrap.unity` → Play → переходить у MainMenu.
   - Натиснути Play на Level 1 → завантажується сцена `Level_01`.
   - Drag+Release на персонажа → летить, фізика працює як у `DragAndLaunch`.
   - Удар об spike → з'являється LevelResult з кнопкою Restart → клік → instant restart (<0.5с).
   - Дотик до finish → LevelResult з Win → Next → Level_02.
3. **Tree Viewer:** `Tools → Controllers → Controllers Hierarchy` під час Play показує живе дерево з `GameRoot → GameLoop → LevelSession → LevelGameplay → Player/Hazards/Finish/Hud`.
4. **Restart cancellation:** додати `Debug.Log` в `OnStop` всіх контролерів `LevelGameplay`-гілки. При restart мають вилогуватись усі. Жодних "забутих" контролерів-зомбі.
5. **CLAUDE.md:** перечитати оновлений документ — гайдлайн самодостатній, з нього зрозуміло як додавати нову фічу.
