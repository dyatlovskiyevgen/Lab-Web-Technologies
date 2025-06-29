//CartViewComponent.cs

using Microsoft.AspNetCore.Mvc;
using OSS.UI.Extentions;
using OSS30333.Domain.Entities;

namespace OSS.UI.Components
{
    public class CartViewComponent : ViewComponent
    {
        //public IViewComponentResult Invoke()
        //{
        //    //var cart = HttpContext.Session.Get<Cart>("cart");
        //    var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart(); //  защита от null
        //    return View();
        //}

        public IViewComponentResult Invoke()
        {
            try
            {
                var cart = HttpContext?.Session?.Get<Cart>("cart") ?? new Cart();
                return View(cart);
            }
            catch (Exception ex)
            {
                // Можно залогировать ошибку, если используешь ILogger
                // _logger.LogError(ex, "Ошибка при загрузке корзины");

                // Возвращаем fallback-представление
                return View("CartError");
            }
        }




    }
}
