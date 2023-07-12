using Microsoft.AspNetCore.Identity;
using RatioShop.Constants;
using RatioShop.Data.Models;

namespace RatioShop.Helpers
{
    public static class ApplicationExtensions
    {
        public static async Task<WebApplication> CreateRolesAsync(this WebApplication app, IConfiguration configuration)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));
            var roles = configuration.GetSection("Roles").Get<List<string>>();

            if(roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            // create anonymous user
            var anonymousUser = new ShopUser()
            {
                Id = UserTest.UserAnonymousID,
                UserName = configuration["RatioSettings:AnonymousUser:UserName"],
                Email = configuration["RatioSettings:AnonymousUser:UserEmail"],
            };
            string anonymousUserPWD = configuration["RatioSettings:AnonymousUser:UserPassword"];
            
            // create user
            var ratioUser = new ShopUser()
            {
                UserName = configuration["RatioSettings:UserName"],
                Email = configuration["RatioSettings:UserEmail"],
            };
            string ratioPWD = configuration["RatioSettings:UserPassword"];

            var userManager = (UserManager<ShopUser>)scope.ServiceProvider.GetService(typeof(UserManager<ShopUser>));
            await CreateUser(userManager, ratioUser, ratioPWD, "SuperAdmin");
            await CreateUser(userManager, anonymousUser, anonymousUserPWD, "Customer");            

            return app;
        }
        private static async Task CreateUser(UserManager<ShopUser>? userManager, ShopUser user, string userPWD, string role)
        {
            if (userManager == null) return;

            var _user = await userManager.FindByEmailAsync(user.Email);            
            if (_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(user, userPWD);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
