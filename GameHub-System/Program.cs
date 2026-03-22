using GameHub.Models;
using GameHub.Service;
using Hub = GameHub.Service.GameHub;

class Program
{
    static void Main(string[] args)
    {
        var projectRoot = Directory.GetCurrentDirectory();
        var dataPath = Path.Combine(projectRoot, "data");

        Console.WriteLine($"Current Directory: {projectRoot}");
        Console.WriteLine($"Data Path: {dataPath}");
        Console.WriteLine($"Data folder exists: {Directory.Exists(dataPath)}\n");

        try
        {
            var hub = new Hub(dataPath);
            var engine = new AchievementEngine(hub);

            hub.AchievementUnlocked += (sender, args) =>
            {
                Console.WriteLine($"Achievement Unlocked! User {args.UserId}: {args.Achievement?.Name} (+{args.Achievement?.Points} pts) - {args.Reason}");
            };

            Console.WriteLine("=== GAME HUB TEST SCENARIO ===\n");

            Console.WriteLine("1. Adding 5 games (different genres)...");
            var games = new List<Game>
            {
                new Game { Id = 1, Title = "Elden Ring", Genre = GameGenre.Action, Price = 59.99m },
                new Game { Id = 2, Title = "Baldur's Gate 3", Genre = GameGenre.RPG, Price = 59.99m },
                new Game { Id = 3, Title = "Civilization VI", Genre = GameGenre.Strategy, Price = 49.99m },
                new Game { Id = 4, Title = "The Legend of Zelda", Genre = GameGenre.Adventure, Price = 69.99m },
                new Game { Id = 5, Title = "Microsoft Flight Simulator", Genre = GameGenre.Simulation, Price = 99.99m }
            };

            foreach (var game in games)
            {
                hub.AddGame(game);
                Console.WriteLine($"  {game.Title} ({game.Genre})");
            }

            Console.WriteLine("\n2. Adding 2 users...");
            var users = new List<User>
            {
                new User { Id = 1, Name = "Alice" },
                new User { Id = 2, Name = "Bob" }
            };

            foreach (var user in users)
            {
                hub.AddUser(user);
                Console.WriteLine($"  {user.Name} (ID: {user.Id})");
            }

            Console.WriteLine("\n3. Adding 3 achievements...");
            var achievements = new List<Achievement>
            {
                new Achievement { Code = "FIRST_SESSION", Name = "First Steps", Points = 10 },
                new Achievement { Code = "HOUR_TOTAL", Name = "Marathon Player", Points = 25 },
                new Achievement { Code = "GENRE_FAN", Name = "Genre Enthusiast", Points = 15 }
            };

            foreach (var achievement in achievements)
            {
                hub.AddAchievement(achievement);
                Console.WriteLine($"  {achievement.Code}: {achievement.Name} ({achievement.Points} pts)");
            }

            Console.WriteLine("\n4. Creating play sessions...");

            Console.WriteLine("  Alice starts Elden Ring...");
            hub.StartSession(1, 1);
            System.Threading.Thread.Sleep(500);
            hub.EndSession(1, 1);
            engine.Evaluate(1);

            Console.WriteLine("  Alice starts Baldur's Gate 3...");
            hub.StartSession(1, 2);
            System.Threading.Thread.Sleep(500);
            hub.EndSession(1, 2);
            engine.Evaluate(1);

            Console.WriteLine("  Alice starts Civilization VI...");
            hub.StartSession(1, 3);
            System.Threading.Thread.Sleep(500);
            hub.EndSession(1, 3);
            engine.Evaluate(1);

            Console.WriteLine("  Bob starts Elden Ring...");
            hub.StartSession(2, 1);
            System.Threading.Thread.Sleep(500);
            hub.EndSession(2, 1);
            engine.Evaluate(2);

            Console.WriteLine("\n5. ANALYTICS BEFORE SAVE:");

            var topUsers = hub.TopUsersByPoints(2);
            Console.WriteLine($"Top 2 Users by Points:");
            foreach (var user in topUsers)
            {
                Console.WriteLine($"  {user.Name}");
            }

            var top3Games = hub.Top3GamesByPlayTime(1);
            Console.WriteLine($"\nTop 3 Games by Play Time (Alice):");
            foreach (var (game, minutes) in top3Games)
            {
                Console.WriteLine($"  {game.Title}: {minutes} minutes");
            }

            var notUnlocked = hub.AchievementsNotUnlocked(1);
            Console.WriteLine($"\nAchievements Not Unlocked (Alice): {notUnlocked.Count}");
            foreach (var achievement in notUnlocked)
            {
                Console.WriteLine($"  {achievement.Code}: {achievement.Name}");
            }

            Console.WriteLine("\n6. Saving data to 'data/' folder...");
            hub.Save(dataPath);
            Console.WriteLine("Data saved successfully");

            Console.WriteLine("\n7. Creating new GameHub and loading saved data...");
            var hubLoaded = new Hub(dataPath);
            hubLoaded.Load(dataPath);
            Console.WriteLine("Data loaded successfully");

            Console.WriteLine("\n8. ANALYTICS AFTER LOAD (VERIFICATION):");

            var topUsersLoaded = hubLoaded.TopUsersByPoints(2);
            Console.WriteLine($"Top 2 Users by Points:");
            foreach (var user in topUsersLoaded)
            {
                Console.WriteLine($"  {user.Name}");
            }

            var top3GamesLoaded = hubLoaded.Top3GamesByPlayTime(1);
            Console.WriteLine($"\nTop 3 Games by Play Time (Alice):");
            foreach (var (game, minutes) in top3GamesLoaded)
            {
                Console.WriteLine($"  {game.Title}: {minutes} minutes");
            }

            var notUnlockedLoaded = hubLoaded.AchievementsNotUnlocked(1);
            Console.WriteLine($"\nAchievements Not Unlocked (Alice): {notUnlockedLoaded.Count}");
            foreach (var achievement in notUnlockedLoaded)
            {
                Console.WriteLine($"  {achievement.Code}: {achievement.Name}");
            }

            Console.WriteLine("\n9. DATA CONSISTENCY CHECK:");
            bool isConsistent =
                topUsers.Count == topUsersLoaded.Count &&
                top3Games.Count == top3GamesLoaded.Count &&
                notUnlocked.Count == notUnlockedLoaded.Count;

            if (isConsistent)
            {
                Console.WriteLine("ALL DATA IS CONSISTENT! Save/Load working correctly.");
            }
            else
            {
                Console.WriteLine("DATA INCONSISTENCY DETECTED!");
            }

            Console.WriteLine("\n=== TEST COMPLETED ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}
