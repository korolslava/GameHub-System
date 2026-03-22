namespace GameHub.Models;

public class Unlock
{
    public int UserId { get; set; }
    public string AchievementCode { get; set; } = string.Empty;
    public DateTime UnlockDate { get; set; }
}