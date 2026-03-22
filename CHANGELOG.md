# Changelog

## [1.2.0] - 2026-03-22
### Added
- Input validation with guard clauses in `GameHubService`
- XML documentation comments on all public methods
- GitHub Actions CI workflow for automated test runs

## [1.1.0] - 2026-03-22
### Added
- `IGameHubService` and `IAchievementEngine` interfaces
- Async `SaveAsync` / `LoadAsync` with `File.WriteAllTextAsync`
- xUnit unit tests (14 test cases)

### Changed
- `RaiseAchievementUnlocked` changed to `internal`
- Removed duplicate telemetry logging

## [1.0.0] - 2026-03-22
### Added
- Core domain models: `Game`, `User`, `PlaySession`, `Achievement`, `Unlock`
- `GameHubService` with in-memory storage and LINQ analytics
- Delegate-based `AchievementEngine` with 3 built-in rules
- Event-driven telemetry pipeline
- JSON persistence (Save/Load)
- `Events/` folder with custom `EventArgs` classes