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
        //AddRoleIfNotExisting("Customer");
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