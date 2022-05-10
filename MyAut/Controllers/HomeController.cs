using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyAut.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyAut.Controllers
{
    //[Authorize(Roles = "admin")]
    public class HomeController : Controller
    {
        
		private ApplicationContext context;

		public HomeController(ApplicationContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Current = context.GetCurrent().Count();
            ViewBag.Total = context.Polls.Count();
            ViewBag.Users = context.Users.Count();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
