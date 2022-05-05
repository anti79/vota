using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAut.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Voting.Controllers
{
	[Authorize]
	public class PollsController:Controller
	{
		public ApplicationContext Context { get; set; }
		public PollsController(ApplicationContext context)
		{
			Context = context;
		}
		public IActionResult Index()
		{
			
			var list = Context.GetCurrent().ToList();

			ViewBag.Polls = list;
			return View();
		}
		public IActionResult Old()
		{

			var list = Context.GetOld().ToList();
			ViewBag.Polls = list;
			return View("Old");
		}
		public IActionResult Future()
		{

			var list = Context.GetPlanned().ToList();
			ViewBag.Polls = list;
			return View("Future");
		}
		[HttpGet("Polls/Vote/{id}")]
		public IActionResult Index(int id)
		{
			if (id == 0) return Old();
			///int id = (int)RouteData.Values["id"];
			//var id = HttpContext.GetRouteData().Values["id"];
			return View("Vote",Context.Polls.Where(p=>p.Id==id).FirstOrDefault());
		}
	}
}
