namespace GameHub.Models;

public enum GameGenre
{
    Action,
    Adventure,
    RPG,
    Strategy,
    Simulation
}

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public GameGenre Genre { get; set; } = GameGenre.Action;
    public decimal Price { get; set; } = 0.0m;
}
