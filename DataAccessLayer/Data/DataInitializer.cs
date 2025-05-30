using DataAccessLayer.Enums;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



namespace DataAccessLayer.Data;

public class DataInitializer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public DataInitializer(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }


    public void SeedData()
    {
        _dbContext.Database.Migrate();

        SeedRoles();
        SeedUsers();
        SeedPlayersAndMatches();
    }

    private void SeedUsers()
    {
        // Password constraints:
        //  - Passwords must have at least one non alphanumeric character.
        //  - Passwords must have at least one digit('0' - '9').
        //  - Passwords must have at least one uppercase('A' - 'Z').
        //  - The password must be at least 6 characters long

        AddUserIfNotExists("admin@angby.se", "*Admin100", new string[] { "Admin" });
        //AddUserIfNotExists("richard.chalk@customer.systementor.se", "Hejsan123#", new string[] { "Customer" });
    }

    private void SeedRoles()
    {
        AddRoleIfNotExisting("Admin");
    }
    
    public void SeedPlayersAndMatches()
    {
        if (_dbContext.Players.Any()) return; // Redan seedat

        var random = new Random();
        var players = new List<Player>
        {
            new Player { FirstName = "Kalle", LastName = "Karlson", Email = "kalle@example.com", Gender = Gender.Male, Birthday = new DateOnly(1985, 5, 15), IsActive = true },
            new Player { FirstName = "Anna", LastName = "Andersson", Email = "anna@example.com", Gender = Gender.Female, Birthday = new DateOnly(1990, 8, 22), IsActive = true },
            new Player { FirstName = "Erik", LastName = "Eriksson", Email = "erik@example.com", Gender = Gender.Male, Birthday = new DateOnly(1988, 3, 10), IsActive = true },
            new Player { FirstName = "Maria", LastName = "Nilsson", Email = "maria@example.com", Gender = Gender.Female, Birthday = new DateOnly(1992, 11, 5), IsActive = true },
            new Player { FirstName = "Johan", LastName = "Johansson", Email = "johan@example.com", Gender = Gender.Male, Birthday = new DateOnly(1987, 7, 18), IsActive = true },
            new Player { FirstName = "Lisa", LastName = "Lindberg", Email = "lisa@example.com", Gender = Gender.Female, Birthday = new DateOnly(1991, 4, 30), IsActive = true },
            new Player { FirstName = "Per", LastName = "Persson", Email = "per@example.com", Gender = Gender.Male, Birthday = new DateOnly(1989, 9, 12), IsActive = true },
            new Player { FirstName = "Sara", LastName = "Svensson", Email = "sara@example.com", Gender = Gender.Female, Birthday = new DateOnly(1993, 6, 25), IsActive = true }
        };

        _dbContext.Players.AddRange(players);
        _dbContext.SaveChanges();

        var playerIds = players.Select(p => p.PlayerId).ToList();
        var matches = new List<Match>();
        var playerStats = new Dictionary<int, (int wins, int losses)>();

        // Initiera statistik för alla spelare
        foreach (var playerId in playerIds)
        {
            playerStats[playerId] = (0, 0);
        }

        // Skapa matcher mellan alla spelare
        for (int i = 0; i < playerIds.Count; i++)
        {
            for (int j = i + 1; j < playerIds.Count; j++)
            {
                // Slumpa antal matcher mellan 6 och 23 för varje spelarpar
                int numberOfMatches = random.Next(6, 23);
                
                for (int m = 0; m < numberOfMatches; m++)
                {
                    var match = new Match
                    {
                        MatchDate = DateTime.Now.AddDays(-random.Next(1, 365)), // Slumpa datum inom senaste året
                        IsSingle = true,
                        IsCompleted = true,
                        MatchType = 1,
                        DurationSeconds = random.Next(20 * 60, 90 * 60), // 20-90 minuter
                        PlayerMatches = new List<PlayerMatch>
                        {
                            new PlayerMatch { PlayerId = playerIds[i], TeamNumber = 1 },
                            new PlayerMatch { PlayerId = playerIds[j], TeamNumber = 2 }
                        },
                        Sets = new List<Set>()
                    };

                    // Skapa 3 set per match
                    for (int setNum = 1; setNum <= 3; setNum++)
                    {
                        // Slumpa vem som vinner setet
                        int setWinner = random.Next(1, 3);
                        int team1Score, team2Score;

                        if (setWinner == 1)
                        {
                            team1Score = random.Next(11, 14); // 11-13 poäng
                            team2Score = random.Next(0, team1Score); // Mindre än team1
                        }
                        else
                        {
                            team2Score = random.Next(11, 14); // 11-13 poäng
                            team1Score = random.Next(0, team2Score); // Mindre än team2
                        }

                        var set = new Set
                        {
                            SetNumber = setNum,
                            Team1Score = team1Score,
                            Team2Score = team2Score,
                            SetWinner = setWinner,
                            IsSetCompleted = true
                        };

                        match.Sets.Add(set);
                    }

                    // Uppdatera match-vinnaren baserat på set-resultaten
                    int team1WonSets = match.Sets.Count(s => s.SetWinner == 1);
                    int team2WonSets = match.Sets.Count(s => s.SetWinner == 2);
                    match.MatchWinner = team1WonSets > team2WonSets ? playerIds[i] : playerIds[j];

                    // Uppdatera statistik
                    var stats1 = playerStats[playerIds[i]];
                    var stats2 = playerStats[playerIds[j]];
                    
                    if (match.MatchWinner == playerIds[i])
                    {
                        stats1.wins++;
                        stats2.losses++;
                    }
                    else
                    {
                        stats2.wins++;
                        stats1.losses++;
                    }
                    
                    playerStats[playerIds[i]] = stats1;
                    playerStats[playerIds[j]] = stats2;

                    matches.Add(match);
                }
            }
        }

        _dbContext.Matches.AddRange(matches);
        _dbContext.SaveChanges();

        // Uppdatera spelarstatistik
        foreach (var player in players)
        {
            var stats = playerStats[player.PlayerId];
            player.NumberOfWins = stats.wins;
            player.NumberOfLosses = stats.losses;
            player.MatchesPlayed = stats.wins + stats.losses;
            player.PlayerWinRatio = player.MatchesPlayed > 0 
                ? (decimal)stats.wins / player.MatchesPlayed * 100 
                : 0;
        }

        _dbContext.SaveChanges();
    }

    private void AddRoleIfNotExisting(string roleName)
    {
        var role = _dbContext.Roles.FirstOrDefault(r => r.Name == roleName);
        if (role == null)
        {
            _dbContext.Roles.Add(new IdentityRole { Name = roleName, NormalizedName = roleName });
            _dbContext.SaveChanges();
        }
    }

    private void AddUserIfNotExists(string userName, string password, string[] roles)
    {
        if (_userManager.FindByEmailAsync(userName).Result != null) return;

        var user = new IdentityUser
        {
            UserName = userName,
            Email = userName,
            EmailConfirmed = true
        };
        _userManager.CreateAsync(user, password).Wait();
        _userManager.AddToRolesAsync(user, roles).Wait();
    }
}