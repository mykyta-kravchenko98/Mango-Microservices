using System.Security.Claims;
using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Initializer.Interfaces;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.Identity.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public void Initializer()
    {
        if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
        }
        else
        {
            return;
        }
        
        ApplicationUser adminUser = new ApplicationUser()
        {
            UserName = "test-admin@gmail.com",
            Email = "test-admin@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "123456789",
            FirstName = "Nikita",
            LastName = "Kravchenko"
        };

        _userManager.CreateAsync(adminUser, "Qwerty123!").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(adminUser, SD.Admin).GetAwaiter().GetResult();

        var adminClaims = _userManager.AddClaimsAsync(adminUser, new[]
        {
            new Claim(JwtClaimTypes.Name, $"{adminUser.FirstName} {adminUser.LastName}"),
            new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
            new Claim(JwtClaimTypes.Role, SD.Admin),
        }).Result;
        
        ApplicationUser customerUser = new ApplicationUser()
        {
            UserName = "test-customer@gmail.com",
            Email = "test-customer@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "1234567890",
            FirstName = "Ben",
            LastName = "Rider"
        };

        _userManager.CreateAsync(customerUser, "Qwerty123!").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(customerUser, SD.Customer).GetAwaiter().GetResult();

        var customerClaims = _userManager.AddClaimsAsync(customerUser, new[]
        {
            new Claim(JwtClaimTypes.Name, $"{customerUser.FirstName} {customerUser.LastName}"),
            new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
            new Claim(JwtClaimTypes.Role, SD.Customer),
        }).Result;
    }
}