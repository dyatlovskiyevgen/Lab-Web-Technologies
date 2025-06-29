//DbInit.cs
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace OSS.UI.Data
{
    public class DbInit
    {
        public static async Task SetupIdentityAdmin(WebApplication application)
        {
            using var scope = application.Services.CreateScope();
            var userManager = scope
                .ServiceProvider
                .GetRequiredService<UserManager<AppUser>>();

            // Проверяем, существует ли пользователь с email "admin@gmail.com"
            var user = await userManager.FindByEmailAsync("admin@gmail.com");
            if (user == null)
            {
                // Создаем нового пользователя
                user = new AppUser();
                await userManager.SetEmailAsync(user, "admin@gmail.com");
                await userManager.SetUserNameAsync(user, user.Email);
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, "123456");

                var claim = new Claim(ClaimTypes.Role, "admin");
                await userManager.AddClaimAsync(user, claim);
            }
        }
    }
}
