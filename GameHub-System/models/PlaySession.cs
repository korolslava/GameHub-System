namespace GameHub.Models;

public class PlaySession
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}