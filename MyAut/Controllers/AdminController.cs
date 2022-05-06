using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAut.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAut.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		ApplicationContext context;
		public AdminController(ApplicationContext context)
		{
			this.context = context;
		}
		public IActionResult Index()
		{
			ViewBag.Polls = context.Polls;
			return View();
		}
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult CreateSave(IDictionary<string,string> data)
		{
			List<Option> options = new();
			for(int i=0;i<int.Parse(data["OptionsCount"]);i++)
			{
				options.Add(new Option() { Name = data["name" + i.ToString()], Description=data["description" + i.ToString()] });
			}
			var poll = new Poll()
			{
				Name = data["Name"],
				Description = data["Description"],
				Options = options
			};

			if (data["StartTime"] != null) poll.StartTime = DateTime.Parse(data["StartTime"]);
			if (data["EndTime"] != null) poll.EndTime = DateTime.Parse(data["EndTime"]);

			context.Polls.Add(poll);
			context.SaveChanges();
			return RedirectToAction("Index");
		}
		[HttpPost("Admin/Delete/{id}")]
		public IActionResult Delete(int id)
		{
			var poll = context.Polls.Where(p => p.Id == id).FirstOrDefault();
			
			try
			{
				poll.Options.RemoveRange(0, poll.OptionsCount);
			}
			catch
			{

			}
			context.Polls.Remove(poll);
			context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
