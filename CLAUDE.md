# PaddleBall Pong — Project Guide for Claude

## Project Overview

This is a Unity 2D Pong game built as an **AI-assisted development lab**. The codebase demonstrates production-quality game architecture using ScriptableObjects as the foundation for an event-driven, loosely-coupled system. The `Core/` folder is a reusable framework; `PaddleBall/` contains the game-specific implementation.

---

## Event-Driven Architecture

### Event Channels (Pub/Sub via ScriptableObjects)

The heart of the architecture is the **Event Channel pattern**: ScriptableObject assets act as event brokers, completely decoupling publishers from subscribers.

**Base types** (in `Core/EventChannels/ScriptableObjects/`):

| Class | Payload | Use case |
|---|---|---|
| `VoidEventChannelSO` | none | Signals (game started, game ended) |
| `GenericEventChannelSO<T>` | `T` | Typed base for all other channels |
| `IntEventChannelSO` | `int` | Score values, counts |
| `FloatEventChannelSO` | `float` | Input axes, timers |
| `StringEventChannelSO` | `string` | Win messages, scene paths |
| `Vector2EventChannelSO` | `Vector2` | Collision normals, directions |
| `BoolEventChannelSO` | `bool` | Pause state |
| `PlayerIDEventChannelSO` | `PlayerIDSO` | Who scored, who won |

**Game-specific channels** (in `PaddleBall/Scripts/ScriptableObjects/`):
- `PlayerScoreEventChannelSO` — broadcasts a `PlayerScore` struct (playerID + score + UI)
- `ScoreListEventChannelSO` — broadcasts `List<PlayerScore>` for batch UI updates

**Publishing** (in any MonoBehaviour or ScriptableObject):
```csharp
[SerializeField] VoidEventChannelSO m_GameStarted;
m_GameStarted.RaiseEvent();

[SerializeField] PlayerIDEventChannelSO m_GoalHit;
m_GoalHit.RaiseEvent(playerIDSO);
```

**Subscribing** (always in `OnEnable`/`OnDisable`):
```csharp
void OnEnable()  => m_GoalHit.OnEventRaised += OnGoalHit;
void OnDisable() => m_GoalHit.OnEventRaised -= OnGoalHit;
```

**Listener components** (for Inspector-driven connections without code):
- `VoidEventChannelListener`, `FloatEventChannelListener`, `Vector2EventChannelListener`, `PlayerIDEventChannelListener`
- Subscribe to a channel asset, invoke a `UnityEvent` response in-Inspector.

---

### Managers

Each manager has a single, clearly bounded responsibility:

| Manager | Responsibility | Key events in | Key events out |
|---|---|---|---|
| `GameManager` | Main game state machine | `m_GoalHit`, `m_ScoreTargetReached`, `m_AllObjectivesCompleted`, `m_GameReset`, `m_IsPaused` | `m_GameStarted`, `m_PointScored`, `m_WinnerShown`, `m_GameEnded` |
| `GameSetup` | Level initialization from SO or JSON | — | — (called by GameManager.Initialize) |
| `ScoreManager` | Score tracking + UI updates | `m_PointScored`, `m_GameStarted`, `m_GameEnded` | `m_ScoreManagerUpdated` |
| `ObjectiveManager` | Win condition tracking | `m_GameStarted`, `m_ObjectiveCompleted` | `m_AllObjectivesCompleted` |
| `UIManager` | Screen navigation with history stack | Various `VoidEventChannelSO` per screen | — |
| `SceneLoader` | Async additive scene load/unload | `m_LoadScenePathEventChannel`, `m_ReloadSceneEventChannel` | — |

---

### Game Loop & Event Flow

```
[Input]
  InputSystem.CallbackContext
    → InputReaderSO.OnMoveP1/P2()
      → P1Moved / P2Moved (float events)
        → Paddle stores input vector

[Physics — FixedUpdate]
  Paddle.FixedUpdate()    — applies movement force (clamped to bounds)
  Ball.FixedUpdate()      — applies pending bounce impulse

[Collision]
  Bouncer.OnCollisionEnter2D()
    → m_BallCollided.RaiseEvent(normal)   → Ball.Bounce()
  ScoreGoal.OnCollisionEnter2D()
    → m_GoalHit.RaiseEvent(playerID)

[Scoring]
  GameManager.OnGoalHit()
    → m_PointScored.RaiseEvent(playerID)
      → ScoreManager.OnPointScored()
          increments Score, updates ScoreView
        → m_ScoreManagerUpdated.RaiseEvent(List<PlayerScore>)
          → ScoreObjectiveSO.UpdateScoreManager()
              if any score >= TargetScore:
                → m_TargetScoreReached.RaiseEvent(playerID)
                → CompleteObjective()
                  → m_ObjectiveCompleted.RaiseEvent()
                    → ObjectiveManager → m_AllObjectivesCompleted.RaiseEvent()

[Win]
  GameManager.OnTargetScoreReached()
    → m_WinnerShown.RaiseEvent(message)   → WinScreen shows text
  GameManager.OnAllObjectivesCompleted()
    → EndGame() → m_GameEnded.RaiseEvent()
      → Ball hides, ScoreManager locks, WinScreen shows
```

---

## Project Conventions

### Naming Rules

Derived from `Core/_StyleGuide/StyleExample.cs`:

| Element | Convention | Example |
|---|---|---|
| Classes / ScriptableObjects | PascalCase; SOs get `SO` suffix | `GameManager`, `GameDataSO` |
| Interfaces | `I` prefix + PascalCase | `ICommand`, `IDataSaver` |
| Member fields | `m_` prefix + PascalCase | `m_GameData`, `m_IsGameOver` |
| Static fields | `s_` prefix + PascalCase | `s_dataSaver` |
| Constants | `k_` prefix + PascalCase | `k_DefaultSubFolder` |
| Local variables / parameters | camelCase | `inputValue`, `playerID` |
| Booleans | Verb prefix | `m_IsGameOver`, `m_HideOnAwake` |
| Methods | PascalCase, verb phrase | `Initialize()`, `RaiseEvent()` |
| Event handlers | `On` prefix | `OnGoalHit()`, `OnGameStarted()` |
| Boolean queries | `Is` / `Has` prefix | `IsPlayer1()`, `HasReachedTargetScore()` |
| Properties | PascalCase, expression-bodied if read-only | `public int Value => m_Value;` |

### Namespace Convention

```
GameSystemsCookbook                       ← Core framework
GameSystemsCookbook.Demos.PaddleBall      ← Game-specific scripts
GameSystemsCookbook.Utilities
GameSystemsCookbook.UI
```

### File Organization

```
Assets/
├── Core/                         # Reusable framework (not game-specific)
│   ├── Audio/
│   │   ├── ScriptableObjects/    # AudioDelegateSO, SimpleAudioDelegateSO
│   │   └── Scripts/              # AudioModifier
│   ├── Commands/
│   │   ├── ScriptableObjects/    # MoveCommandSO
│   │   └── Scripts/              # ICommand, CommandManager
│   ├── EventChannels/
│   │   ├── ScriptableObjects/    # GenericEventChannelSO + all typed variants
│   │   ├── Scripts/              # Listener components
│   │   └── Editor/               # Custom inspectors
│   ├── Objectives/
│   │   ├── ScriptableObjects/    # ObjectiveSO
│   │   └── Scripts/              # ObjectiveManager
│   ├── RuntimeSets/
│   │   ├── ScriptableObjects/    # RuntimeSetSO<T>
│   │   └── Scripts/
│   ├── SceneManagement/
│   │   ├── Scripts/              # SceneLoader, SequenceManager
│   │   └── Editor/               # SceneBootstrapper
│   ├── UI/
│   │   ├── ScriptableObjects/    # PlayerIDSO, CreditsSO
│   │   └── Scripts/              # UIManager, View, screen subclasses
│   ├── Utilities/
│   │   ├── ScriptableObjects/    # DescriptionSO
│   │   └── Scripts/              # NullRefChecker, SaveManager, JsonSaver, etc.
│   └── _StyleGuide/              # StyleExample.cs — authoritative naming reference
│
└── PaddleBall/                   # Game-specific code
    ├── Scripts/
    │   ├── ScriptableObjects/    # GameDataSO, LevelLayoutSO, InputReaderSO, ...
    │   ├── Managers/             # GameManager, GameSetup, ScoreManager
    │   └── UI/                   # ScoreView, WinScreen
    ├── Input/                    # PaddleBallControls (auto-generated)
    ├── Prefabs/
    │   ├── Gameplay/
    │   ├── Level/
    │   ├── UI/
    │   ├── Cameras/
    │   └── Sounds/
    ├── Data/                     # SO asset instances
    │   ├── EventChannels/
    │   │   ├── Gameplay/
    │   │   ├── SceneManagement/
    │   │   └── UI/
    │   ├── GameData/
    │   ├── Input/
    │   ├── LevelLayouts/
    │   ├── Objectives/
    │   └── PlayerID/
    └── Scenes/
        ├── Bootloader_Scene.unity
        └── SampleScene.unity
```

### Design Patterns

| Pattern | Where it's used |
|---|---|
| **Event Channel (Pub/Sub)** | All cross-system communication via `EventChannelSO` assets |
| **ScriptableObject as data container** | `GameDataSO`, `LevelLayoutSO`, `PlayerIDSO` |
| **ScriptableObject as event broker** | All `*EventChannelSO` assets |
| **Dependency Injection (method)** | `Initialize(GameDataSO, InputReaderSO)` throughout |
| **Strategy** | `AudioDelegateSO` / `SimpleAudioDelegateSO` + `AudioModifier` |
| **Command** | `ICommand`, `MoveCommandSO`, `CommandManager` |
| **Composite** | `ObjectiveManager` over `ObjectiveSO[]`; `UIManager` over `View[]` |
| **Factory (config-driven)** | `GameSetup.CreateBall/Paddle/Walls/Goals()` from SO data |
| **Runtime Set** | `RuntimeSetSO<T>` tracks live scene objects |

---

## Key ScriptableObjects

### `GameDataSO` — Gameplay Configuration Hub
**Path:** `PaddleBall/Scripts/ScriptableObjects/GameDataSO.cs`
**Create menu:** `GameSystemsCookbook/Data/GameData`

Central configuration for all physics and gameplay tuning. Passed to `GameManager.Initialize()` and forwarded to every gameplay component.

| Field | Type | Purpose |
|---|---|---|
| `PaddleSpeed` | `float` | Force applied per frame to paddles |
| `PaddleLinearDrag` | `float` | Rigidbody2D drag for paddles |
| `PaddleMass` | `float` | Rigidbody2D mass for paddles |
| `BallSpeed` | `float` | Initial launch force for ball |
| `MaxSpeed` | `float` | Speed cap applied after bounce |
| `BounceMultiplier` | `float` | Force scalar applied on ball bounce |
| `DelayBetweenPoints` | `float` | Seconds before re-serving after goal |
| `Player1` / `Player2` | `PlayerIDSO` | Identity markers for each player |
| `LevelLayout` | `LevelLayoutSO` | Which level layout to use |
| `P1Sprite` / `P2Sprite` | `Sprite` | Optional per-player paddle sprites |

Helper methods: `IsPlayer1(PlayerIDSO)`, `IsPlayer2(PlayerIDSO)`.

---

### `LevelLayoutSO` — Level Geometry Data
**Path:** `PaddleBall/Scripts/ScriptableObjects/LevelLayoutSO.cs`
**Create menu:** `GameSystemsCookbook/Data/LevelLayout`

Stores all spawn positions and wall/goal layout. Supports round-tripping to JSON for runtime level loading.

| Field | Type | Purpose |
|---|---|---|
| `BallStartPosition` | `TransformSaveData` | Ball spawn transform |
| `Paddle1StartPosition` | `Paddle2StartPosition` | `TransformSaveData` | Paddle spawns |
| `Goal1` / `Goal2` | `TransformSaveData` | Goal trigger zone transforms |
| `LevelWalls[]` | `TransformSaveData[]` | Wall positions/scales |
| `LevelPrefab` | `GameObject` | Optional prefab for level visuals |
| `JsonFilename` | `string` | Filename for `ExportToJson()` / JSON load mode |

`TransformSaveData` struct: `{ Vector3 position, rotation, localScale }`.

`ExportToJson()` serializes to `Application.persistentDataPath/Saves/` via `SaveManager`.
`GameSetup` can load from either the SO directly or the exported JSON (`InitializeMode.ScriptableObject` vs `InitializeMode.Json`).

---

### `InputReaderSO` — Input System Relay
**Path:** `PaddleBall/Scripts/ScriptableObjects/InputReaderSO.cs`
**Create menu:** `GameSystemsCookbook/Input/InputReader`

Bridges Unity's new Input System to the event-channel architecture. Implements `PaddleBallControls.IGameplayActions` (auto-generated interface). Exposes plain `UnityAction` events so gameplay code never imports `UnityEngine.InputSystem` directly.

| Event | Type | Trigger |
|---|---|---|
| `P1Moved` | `UnityAction<float>` | W (positive) / S (negative) |
| `P2Moved` | `UnityAction<float>` | Up Arrow / Down Arrow |
| `GameRestarted` | `UnityAction` | R key |

Lifecycle: `OnEnable` creates a `PaddleBallControls` instance, enables the Gameplay action map, and subscribes. `OnDisable` unsubscribes.

---

### `ScoreObjectiveSO` — Win Condition
**Path:** `PaddleBall/Scripts/ScriptableObjects/ScoreObjectiveSO.cs`

Extends `ObjectiveSO`. Listens to `m_ScoreManagerUpdated`. When any player's score reaches `TargetScore`, calls `CompleteObjective()` (inherited) and broadcasts the winner via `m_TargetScoreReached`.

---

### `PlayerIDSO` — Player Identity Marker
**Path:** `Core/UI/ScriptableObjects/PlayerIDSO.cs`

An intentionally empty ScriptableObject. Two assets (`Player1`, `Player2`) are created and used as unique identity tokens throughout the system — passed between components, compared by reference (`==`), broadcast on event channels.

---

## Initialization Sequence

```
Bootloader_Scene loads
  └─ GameManager.Awake()
       └─ Initialize()
            ├─ NullRefChecker.Validate(this)      ← catches missing SO references early
            └─ GameSetup.Initialize(GameDataSO, InputReaderSO)
                 ├─ CreateBall()   → ball.Initialize(GameDataSO)
                 ├─ CreatePaddles() → paddle.Initialize(GameDataSO, PlayerIDSO, InputReaderSO)
                 ├─ CreateWalls()  → instantiate from LevelLayout.LevelWalls[]
                 └─ CreateGoals()  → instantiate, assign PlayerIDSO (reversed per goal)
  └─ GameManager.Start()
       └─ StartGame() [if m_AutoStart = true]
            └─ m_GameStarted.RaiseEvent()
                 ├─ Ball.BeginGamePlay()   → starts ServeAfterDelay coroutine
                 ├─ ScoreManager.InitializeScores()
                 └─ ObjectiveManager.OnGameStarted() → resets all objectives
```

---

## Validation & Safety

- **`NullRefChecker.Validate(this)`** — called in `Awake`/`OnEnable` on all major components. Uses reflection to find `[SerializeField]` fields; logs errors for any that are null. Mark optional fields with `[Optional]` to suppress.
- All `EventChannelSO` subscriptions use `OnEnable`/`OnDisable` to prevent stale listeners across domain reloads.
- `InputReaderSO` is a ScriptableObject (not a MonoBehaviour) so it survives scene transitions without `DontDestroyOnLoad`.
