/*HomeController.cs*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OSS.UI.Models;
using Serilog;

namespace OSS.UI.Controllers
{
    public class HomeController : Controller
    {
        //************Для выпадающего списка********************
        private readonly List<ListDemo> _listData;
        public HomeController()
        {            
            _listData = new List<ListDemo>
            {
                new ListDemo {Id=1, Name="Item 1"},
                new ListDemo {Id=2, Name="Item 2"},
                new ListDemo {Id=3, Name="Item 3"}
            };
        }
        //************Для выпадающего списка (конец)********************

        public IActionResult Index()
        {
            ViewData["Title"] = "Index page";
            ViewData["LabWorkTitle"] = "Лабораторная работа №1-8"; 
            
            ViewData["Lst"] = new SelectList(_listData, "Id", "Name");

            // Log.Information("Hello из метода Index контроллера Home!");
            return View();
        }
    }
}
