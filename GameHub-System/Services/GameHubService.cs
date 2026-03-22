using System.Text.Json;

using GameHub.Models;
namespace GameHub.Service;

public class GameHub
{
    public delegate bool AchievementRule(GameHub hub, int userId, out string reason);

    List<Game> _games;
    List<User> _users;
    List<PlaySession> _playSessions;
    List<Achievement> _achievements;
    List<Unlock> _unlocks;
    List<TelemetryEvent> _telemetry;

    string _dataFolderPath;

    const string FileNameGames = "games.json";
    const string FileNameUsers = "users.json";
    const string FileNamePlaySessions = "playSessions.json";
    const string FileNameAchievements = "achievements.json";
    const string FileNameUnlocks = "unlocks.json";
    const string FileNameTelemetry = "telemetry.json";

    public event EventHandler<SessionEventArgs> SessionStarted;
    public event EventHandler<SessionEventArgs> SessionEnded;
    public event EventHandler<AchievementUnlockedEventArgs> AchievementUnlocked;

    public GameHub(string dataFolderPath = "data")
    {
        _dataFolderPath = dataFolderPath;
        if (!Directory.Exists(_dataFolderPath))
        {
            Directory.CreateDirectory(_dataFolderPath);
        }

        _games = new List<Game>();
        _users = new List<User>();
        _playSessions = new List<PlaySession>();
        _achievements = new List<Achievement>();
        _unlocks = new List<Unlock>();
        _telemetry = new List<TelemetryEvent>();
    }

    public void Save(string folderPath)
    {
        _dataFolderPath = folderPath;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        void WriteJsonFile(string filePath, string jsonContent)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(jsonContent);
            }
        }

        var gameFilePath = Path.Combine(folderPath, FileNameGames);
        var gamesJson = JsonSerializer.Serialize(_games, options);
        WriteJsonFile(gameFilePath, gamesJson);

        var userFilePath = Path.Combine(folderPath, FileNameUsers);
        var usersJson = JsonSerializer.Serialize(_users, options);
        WriteJsonFile(userFilePath, usersJson);

        var sessionFilePath = Path.Combine(folderPath, FileNamePlaySessions);
        var sessionsJson = JsonSerializer.Serialize(_playSessions, options);
        WriteJsonFile(sessionFilePath, sessionsJson);

        var achievementFilePath = Path.Combine(folderPath, FileNameAchievements);
        var achievementsJson = JsonSerializer.Serialize(_achievements, options);
        WriteJsonFile(achievementFilePath, achievementsJson);

        var unlockFilePath = Path.Combine(folderPath, FileNameUnlocks);
        var unlocksJson = JsonSerializer.Serialize(_unlocks, options);
        WriteJsonFile(unlockFilePath, unlocksJson);

        var telemetryFilePath = Path.Combine(folderPath, FileNameTelemetry);
        var telemetryJson = JsonSerializer.Serialize(_telemetry, options);
        WriteJsonFile(telemetryFilePath, telemetryJson);
    }

    public void Load(string folderPath)
    {
        _dataFolderPath = folderPath;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            return;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var gameFilePath = Path.Combine(folderPath, FileNameGames);
        if (File.Exists(gameFilePath))
        {
            var gamesJson = File.ReadAllText(gameFilePath);
            _games = JsonSerializer.Deserialize<List<Game>>(gamesJson, options) ?? new List<Game>();
        }

        var userFilePath = Path.Combine(folderPath, FileNameUsers);
        if (File.Exists(userFilePath))
        {
            var usersJson = File.ReadAllText(userFilePath);
            _users = JsonSerializer.Deserialize<List<User>>(usersJson, options) ?? new List<User>();
        }

        var sessionFilePath = Path.Combine(folderPath, FileNamePlaySessions);
        if (File.Exists(sessionFilePath))
        {
            var sessionsJson = File.ReadAllText(sessionFilePath);
            _playSessions = JsonSerializer.Deserialize<List<PlaySession>>(sessionsJson, options) ?? new List<PlaySession>();
        }

        var achievementFilePath = Path.Combine(folderPath, FileNameAchievements);
        if (File.Exists(achievementFilePath))
        {
            var achievementsJson = File.ReadAllText(achievementFilePath);
            _achievements = JsonSerializer.Deserialize<List<Achievement>>(achievementsJson, options) ?? new List<Achievement>();
        }

        var unlockFilePath = Path.Combine(folderPath, FileNameUnlocks);
        if (File.Exists(unlockFilePath))
        {
            var unlocksJson = File.ReadAllText(unlockFilePath);
            _unlocks = JsonSerializer.Deserialize<List<Unlock>>(unlocksJson, options) ?? new List<Unlock>();
        }

        var telemetryFilePath = Path.Combine(folderPath, FileNameTelemetry);
        if (File.Exists(telemetryFilePath))
        {
            var telemetryJson = File.ReadAllText(telemetryFilePath);
            _telemetry = JsonSerializer.Deserialize<List<TelemetryEvent>>(telemetryJson, options) ?? new List<TelemetryEvent>();
        }
    }

    private void LogTelemetry(string eventType, int userId, string details = "")
    {
        var telemetryEvent = new TelemetryEvent
        {
            EventType = eventType,
            UserId = userId,
            Timestamp = DateTime.Now,
            Details = details
        };
        _telemetry.Add(telemetryEvent);
    }

    public void AddGame(Game game)
    {
        _games.Add(game);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(_games, options);
        var filePath = Path.Combine(_dataFolderPath, FileNameGames);
        File.WriteAllText(filePath, jsonString);
    }

    public void AddUser(User user)
    {
        _users.Add(user);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(_users, options);
        var filePath = Path.Combine(_dataFolderPath, FileNameUsers);
        File.WriteAllText(filePath, jsonString);
    }

    public void AddAchievement(Achievement achievement)
    {
        _achievements.Add(achievement);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(_achievements, options);
        var filePath = Path.Combine(_dataFolderPath, FileNameAchievements);
        File.WriteAllText(filePath, jsonString);
    }

    public void AddUnlock(Unlock unlock)
    {
        _unlocks.Add(unlock);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(_unlocks, options);
        var filePath = Path.Combine(_dataFolderPath, FileNameUnlocks);
        File.WriteAllText(filePath, jsonString);

        LogTelemetry("Unlock", unlock.UserId, unlock.AchievementCode);
    }

    public void StartSession(int userId, int gameId)
    {
        var session = new PlaySession
        {
            UserId = userId,
            GameId = gameId,
            StartTime = DateTime.Now
        };
        _playSessions.Add(session);

        var args = new SessionEventArgs { UserId = userId, GameId = gameId };
        SessionStarted?.Invoke(this, args);

        LogTelemetry("Start", userId, $"GameId: {gameId}");

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(_playSessions, options);
        var filePath = Path.Combine(_dataFolderPath, FileNamePlaySessions);
        File.WriteAllText(filePath, jsonString);
    }

    public void EndSession(int userId, int gameId)
    {
        var session = _playSessions.FindLast(s => s.UserId == userId && s.GameId == gameId && s.EndTime == default);
        if (session != null)
        {
            session.EndTime = DateTime.Now;

            var args = new SessionEventArgs { UserId = userId, GameId = gameId };
            SessionEnded?.Invoke(this, args);

            LogTelemetry("End", userId, $"GameId: {gameId}");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(_playSessions, options);
            var filePath = Path.Combine(_dataFolderPath, FileNamePlaySessions);
            File.WriteAllText(filePath, jsonString);
        }
    }
}

public class SessionEventArgs
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
}

public class AchievementUnlockedEventArgs
{
    public int UserId { get; set; }
    public string AchievementCode { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
    public Achievement? Achievement { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class TelemetryEvent
{
    public string EventType { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}