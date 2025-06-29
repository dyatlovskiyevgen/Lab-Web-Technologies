using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using OSS.UI.Data;

namespace OSS.UI.Controllers
{
    //public class ImageController (UserManager<AppUser>userManager) : Controller
    //{
    //    public async Task<IActionResult> GetAvatar()
    //    {
    //        var email = User.FindFirst(ClaimTypes.Email)!.Value;
    //        var user = await userManager.FindByEmailAsync(email);
    //        if (user == null)
    //        {
    //            return NotFound();
    //        }
    //        if (user.Avatar != null)
    //            return File(user.Avatar, user.MimeType);

    //        var imagePath = Path.Combine("Images", "avatar_default.jpg");     
    //        return File(imagePath, "image/jpeg");
    //    }
    //}

    [Authorize]
    public class ImageController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ImageController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> GetAvatar()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound();

            // Логирование для отладки
            System.Diagnostics.Debug.WriteLine($"User: {user.Email}, Avatar: {user.Avatar?.Length ?? 0} bytes, Mime: {user.MimeType}");

            if (user.Avatar != null && !string.IsNullOrEmpty(user.MimeType))
                return File(user.Avatar, user.MimeType);

            var defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "avatar_default.jpg");
            return PhysicalFile(defaultPath, "image/jpeg");
        }
    }
}
