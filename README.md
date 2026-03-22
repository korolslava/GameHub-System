# 🎮 GameHub System

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/language-C%23-239120?logo=csharp)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Tests](https://github.com/korolslava/GameHub-System/actions/workflows/tests.yml/badge.svg)](https://github.com/korolslava/GameHub-System/actions/workflows/tests.yml)
[![Patterns](https://img.shields.io/badge/patterns-Observer%20%7C%20Rule%20Engine%20%7C%20Service%20Layer-orange)]()
[![License](https://img.shields.io/badge/license-MIT-green)]()

## 📖 Overview

**GameHub System** is an in-memory game library management system built in C# (.NET 8). It models a real-world gaming platform with user tracking, play sessions, a delegate-based achievement engine, and a full telemetry pipeline — all persisted to and restored from structured JSON files asynchronously.

The project demonstrates practical command of core C# and .NET concepts:

| Pillar | What's covered |
|---|---|
| **Domain Modeling** | Strongly typed entities, enums, value defaults |
| **Collections & LINQ** | Aggregations, projections, set operations, ordering |
| **Events & Telemetry** | `EventHandler<TEventArgs>`, custom `EventArgs`, event log |
| **Delegate Rule Engine** | `delegate bool AchievementRule(...)`, pluggable rule registry |
| **Interfaces** | `IGameHubService`, `IAchievementEngine` — decoupled contracts |
| **Async I/O** | `SaveAsync` / `LoadAsync` via `File.WriteAllTextAsync` |
| **JSON Persistence** | `System.Text.Json`, `static readonly` options, safe load |
| **Unit Testing** | xUnit, 8 test cases covering core business logic |

---

## 🏗️ Architecture

```
GameHub-System/
├── Models/
│   ├── Game.cs                  # Game entity (Id, Title, Genre, Price)
│   ├── User.cs                  # User entity (Id, Name)
│   ├── PlaySession.cs           # Session (UserId, GameId, StartTime, EndTime)
│   ├── Achievement.cs           # Achievement (Code, Name, Points)
│   └── Unlock.cs                # Unlock record (UserId, AchievementCode, UnlockDate)
│
├── Events/
│   ├── SessionEventArgs.cs              # UserId, GameId, Time
│   ├── AchievementUnlockedEventArgs.cs  # UserId, Achievement, Reason, Time
│   └── TelemetryEvent.cs               # EventType, UserId, Timestamp, Details
│
├── Interfaces/
│   ├── IGameHubService.cs       # Contract for core service
│   └── IAchievementEngine.cs    # Contract for rule engine
│
├── Services/
│   ├── GameHubService.cs        # Core service — storage, LINQ, events, async Save/Load
│   └── AchievementEngine.cs     # Delegate-based rule engine
│
├── data/                        # JSON persistence folder
│   ├── games.json
│   ├── users.json
│   ├── playSessions.json
│   ├── achievements.json
│   ├── unlocks.json
│   └── telemetry.json
│
└── Program.cs                   # Entry point & demonstration scenario

GameHub.Tests/
└── GameHubServiceTests.cs       # xUnit unit tests (8 test cases)
```

---

## 🛠️ Technology Stack

- **Runtime:** .NET 8.0
- **Language:** C# 12
- **Storage:** In-memory (`List<T>`, `Dictionary<K,V>`)
- **Serialization:** `System.Text.Json` with `static readonly JsonSerializerOptions`
- **Async I/O:** `File.WriteAllTextAsync` / `File.ReadAllTextAsync`
- **Testing:** xUnit
- **Patterns:** Service Layer, Rule Engine (Delegate), Observer (Events)

---

## 📦 Domain Models

### `Game`
```csharp
public class Game
{
    public int       Id    { get; set; }
    public string    Title { get; set; }
    public GameGenre Genre { get; set; }
    public decimal   Price { get; set; }
}
```

### `User`
```csharp
public class User
{
    public int    Id   { get; set; }
    public string Name { get; set; }
}
```

### `PlaySession`
```csharp
public class PlaySession
{
    public int      UserId    { get; set; }
    public int      GameId    { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime   { get; set; }  // default while active
}
```

### `Achievement`
```csharp
public class Achievement
{
    public string Code   { get; set; }
    public string Name   { get; set; }
    public int    Points { get; set; }
}
```

---

## ⚙️ GameHubService

Implements `IGameHubService`. Central service responsible for all storage, business logic, events, and persistence.

### Key Methods

| Method | Description |
|---|---|
| `AddGame(game)` | Adds a game to the in-memory store |
| `AddUser(user)` | Registers a new user |
| `AddAchievement(achievement)` | Registers a new achievement |
| `StartSession(userId, gameId)` | Opens a play session, fires `SessionStarted` |
| `EndSession(userId, gameId)` | Closes the last active session, fires `SessionEnded` |
| `UnlockAchievement(userId, code)` | Unlocks achievement if not already unlocked |
| `SaveAsync(folderPath)` | Serializes all state to 6 JSON files asynchronously |
| `LoadAsync(folderPath)` | Restores all state from JSON files asynchronously |

### LINQ Analytics

| Method | Returns |
|---|---|
| `TotalMinutesByGenre(userId)` | `Dictionary<string, int>` — playtime grouped by genre |
| `Top3GamesByPlayTime(userId)` | `List<(Game, int minutes)>` — top 3 most played games |
| `TopUsersByPoints(topN)` | `List<User>` — top N users ranked by achievement points |
| `AchievementsNotUnlocked(userId)` | `List<Achievement>` — locked achievements for user |

---

## 📡 Events & Telemetry

```csharp
public event EventHandler<SessionEventArgs>             SessionStarted;
public event EventHandler<SessionEventArgs>             SessionEnded;
public event EventHandler<AchievementUnlockedEventArgs> AchievementUnlocked;
```

All significant state changes fire typed events. Every `Start`, `End`, and `Unlock` action is automatically logged to `telemetry.json` as a chronological audit trail.

---

## 🏆 Achievement Engine

Implements `IAchievementEngine`. Uses a `Dictionary<string, AchievementRule>` to evaluate pluggable delegate-based rules.

### Rule Delegate
```csharp
public delegate bool AchievementRule(GameHubService hub, int userId, out string reason);
```

### Built-in Rules

| Code | Name | Condition |
|---|---|---|
| `FIRST_SESSION` | First Steps | User completed at least 1 play session |
| `HOUR_TOTAL` | Marathon Player | Total playtime ≥ 60 minutes |
| `GENRE_FAN` | Genre Enthusiast | ≥ 3 sessions in the same genre |

New rules can be added without modifying `GameHubService` — fully open for extension.

---

## 💾 Async JSON Persistence

```csharp
// Save entire state
await hub.SaveAsync("data/");

// Restore entire state
await hub.LoadAsync("data/");
```

- Uses `File.WriteAllTextAsync` / `File.ReadAllTextAsync`
- `static readonly JsonSerializerOptions` — reused across all calls
- Safe load: missing files are silently skipped, collections default to empty
- Generic local function `ReadJson<T>` eliminates repetition in `LoadAsync`

---

## 🧪 Unit Tests (xUnit)

8 test cases covering core business logic:

| Test | What it verifies |
|---|---|
| `AddGame_ShouldStoreGame` | Game is stored and retrievable by Id |
| `StartSession_And_EndSession_ShouldCreateCompletedSession` | Session has EndTime after EndSession |
| `TopUsersByPoints_ShouldReturnUsersOrderedByPoints` | Ranking is correct |
| `AchievementsNotUnlocked_ShouldReturnOnlyLocked` | Locked filter works correctly |
| `HasUserUnlockedAchievement_ShouldReturnTrueAfterUnlock` | Unlock is persisted |
| `UnlockAchievement_ShouldNotDuplicate` | Second unlock is ignored |
| `SaveAsync_And_LoadAsync_ShouldPersistData` | Data survives save/load cycle |

```bash
dotnet test
# Passed: 8, Failed: 0
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Run

```bash
git clone https://github.com/korolslava/GameHub-System.git
cd GameHub-System
dotnet run --project GameHub-System
```

### Run Tests

```bash
dotnet test
```

---

## 🧠 Key Design Decisions

- **`static readonly JsonSerializerOptions`** — single instance reused across all serialization calls instead of allocating a new object per method
- **`internal RaiseAchievementUnlocked`** — event firing is encapsulated, not exposed as a public API
- **Telemetry logged once** — unlock is logged in `AddUnlock`, not duplicated in the event handler
- **Generic `ReadJson<T>` local function** — eliminates 6 identical deserialize blocks in `LoadAsync`
- **Interface contracts** — `IGameHubService` and `IAchievementEngine` decouple consumers from implementation, enabling unit testing and future substitution

## 📋 Changelog
See [CHANGELOG.md](CHANGELOG.md) for version history.
