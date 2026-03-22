using GameHub.Models;
using static GameHub.Service.GameHub;
namespace GameHub.Service;

public class AchievementEngine
{
    Dictionary<string, AchievementRule> _rules;
    GameHub _hub;

    public AchievementEngine(GameHub hub)
    {
        _hub = hub;
        _rules = new Dictionary<string, AchievementRule>
        {
            { "FIRST_SESSION", IsFirstSession },
            { "HOUR_TOTAL", HasTotalHourPlayed },
            { "GENRE_FAN", IsGenreFan }
        };
    }

    public void Evaluate(int userId)
    {
        foreach (var rule in _rules)
        {
            if (rule.Value(_hub, userId, out string reason))
            {
                var achievement = _hub.GetAchievementByCode(rule.Key);
                if (achievement != null && !_hub.HasUserUnlockedAchievement(userId, rule.Key))
                {
                    _hub.UnlockAchievement(userId, rule.Key);
                    _hub.RaiseAchievementUnlocked(this, new AchievementUnlockedEventArgs
                    {
                        UserId = userId,
                        Achievement = achievement,
                        Reason = reason
                    });
                }
            }
        }
    }

    private bool IsFirstSession(GameHub hub, int userId, out string reason)
    {
        reason = string.Empty;
        var userSessions = hub.GetUserSessions(userId);
        if (userSessions.Count == 1)
        {
            reason = "User completed their first session";
            return true;
        }
        return false;
    }

    private bool HasTotalHourPlayed(GameHub hub, int userId, out string reason)
    {
        reason = string.Empty;
        var userSessions = hub.GetUserSessions(userId);
        int totalMinutes = 0;

        foreach (var session in userSessions)
        {
            if (session.EndTime != default)
            {
                totalMinutes += (int)(session.EndTime - session.StartTime).TotalMinutes;
            }
        }

        if (totalMinutes >= 60)
        {
            reason = $"User has played for {totalMinutes} minutes (≥ 60 minutes)";
            return true;
        }
        return false;
    }

    private bool IsGenreFan(GameHub hub, int userId, out string reason)
    {
        reason = string.Empty;
        var genreMinutes = hub.TotalMinutesByGenre(userId);

        foreach (var genre in genreMinutes)
        {
            var userSessions = hub.GetUserSessions(userId);
            var genreSessionCount = 0;

            foreach (var session in userSessions)
            {
                var game = hub.GetGameById(session.GameId);
                if (game != null && game.Genre.ToString() == genre.Key)
                {
                    genreSessionCount++;
                }
            }

            if (genreSessionCount >= 3)
            {
                reason = $"User has {genreSessionCount} sessions in {genre.Key} genre";
                return true;
            }
        }
        return false;
    }
}