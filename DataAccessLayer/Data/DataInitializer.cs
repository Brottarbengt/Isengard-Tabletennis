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

        AddUserIfNotExists("admin@angby.com", "*Admin100", new string[] { "Admin" });
    }

    private void SeedRoles()
    {
        AddRoleIfNotExisting("Admin");
    }

    public void SeedPlayersAndMatches()
    {
        if (_dbContext.Players.Any()) return; // Redan seedat

        // 1. Skapa spelare
        //var players = new List<Player>
        //{
        //    new Player { FirstName = "Alice", LastName = "Andersson", Email = "alice@example.com", Gender = Gender.Female },
        //    new Player { FirstName = "Bob", LastName = "Bengtsson", Email = "bob@example.com", Gender = Gender.Male },
        //    new Player { FirstName = "Clara", LastName = "Carlsson", Email = "clara@example.com", Gender = Gender.Female },
        //    new Player { FirstName = "David", LastName = "Dahl", Email = "david@example.com", Gender = Gender.Male }
        //};

        var players = new List<Player>
            {
                new Player
                {
                    FirstName = "Alice",
                    LastName = "Andersson",
                    Email = "alice@example.com",
                    Gender = Gender.Female,
                    Birthday = new DateOnly(1995, 4, 12)
                },
                new Player
                {
                    FirstName = "Bob",
                    LastName = "Bengtsson",
                    Email = "bob@example.com",
                    Gender = Gender.Male,
                    Birthday = new DateOnly(1992, 11, 23)
                },
                new Player
                {
                    FirstName = "Clara",
                    LastName = "Carlsson",
                    Email = "clara@example.com",
                    Gender = Gender.Female,
                    Birthday = new DateOnly(1998, 6, 5)
                },
                new Player
                {
                    FirstName = "David",
                    LastName = "Dahl",
                    Email = "david@example.com",
                    Gender = Gender.Male,
                    Birthday = new DateOnly(1990, 2, 14)
                }
            };

        _dbContext.Players.AddRange(players);
        _dbContext.SaveChanges();

        var playerIds = players.Select(p => p.PlayerId).ToList();
        int matchCounter = 1;

        // 2. Skapa matcher (alla möter alla en gång)
        for (int i = 0; i < playerIds.Count; i++)
        {
            for (int j = i + 1; j < playerIds.Count; j++)
            {
                var player1Id = playerIds[i];
                var player2Id = playerIds[j];

                var match = new Match
                {
                    MatchDate = DateTime.Now.AddDays(-matchCounter),
                    IsSingle = true,
                    IsCompleted = true,
                    MatchType = 1,
                    MatchWinner = player1Id, // godtycklig vinnare
                    PlayerMatches = new List<PlayerMatch>
                    {
                        new PlayerMatch { PlayerId = player1Id, TeamNumber = 1 },
                        new PlayerMatch { PlayerId = player2Id, TeamNumber = 2 }
                    },
                    Sets = new List<Set>()
                };

                // 3 set per match (bäst av 3)
                for (int setNum = 1; setNum <= 3; setNum++)
                {
                    var team1Score = 11;
                    var team2Score = setNum == 3 ? 9 : 7; // Lite variation

                    var set = new Set
                    {
                        SetNumber = setNum,
                        Team1Score = team1Score,
                        Team2Score = team2Score,
                        SetWinner = 1,
                        IsSetCompleted = true,
                        SetInfo = new SetInfo
                        {
                            InfoMessage = $"Set {setNum} mellan spelare {player1Id} och {player2Id}",
                            IsPlayer1Serve = setNum % 2 == 1,
                            IsPlayer1StartServer = setNum == 1,
                            ServeCounter = 4
                        }
                    };

                    match.Sets.Add(set);
                }

                _dbContext.Matches.Add(match);
                matchCounter++;
            }
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