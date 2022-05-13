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
				Options = options,
				Image = data["Image"]
			};

			if (data["StartTime"] != null) poll.StartTime = DateTime.Parse(data["StartTime"]);
			if (data["EndTime"] != null) poll.EndTime = DateTime.Parse(data["EndTime"]);

			if(!poll.Validate()) return BadRequest();

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
				poll.Options.RemoveAll(x => true);
				poll.Votes.RemoveAll(x => true);
			}
			catch
			{

			}
			context.Polls.Remove(poll);
			context.SaveChanges();
			return RedirectToAction("Index");
		}


		[HttpGet("Admin/Edit/{id}")]
		public IActionResult Edit(int id)
		{
			var poll = context.Polls.Where(p => p.Id == id).FirstOrDefault();
			return View("Edit", poll);
		}
		[HttpPost]
		public IActionResult EditSave(IDictionary<string, string> data)
		{
			var poll = context.Polls.Where(p => p.Id == int.Parse(data["PollId"])).FirstOrDefault();
			int count = int.Parse(data["OptionsCount"]);
			if (poll.OptionsCount < count) {
				for (int i = 0; i < count - poll.OptionsCount;i++) poll.Options.Add(new Option { });
			}				
			for (int i = 0; i < count; i++)
			{
				var namei = "name" + i.ToString();
				var desci = "description" + i.ToString();
				var imgi = "img" + i.ToString();
				poll.Options[i].Name = data[namei];
				poll.Options[i].Description = data[desci];
				poll.Options[i].Image = data[imgi];

			}

			poll.Name = data["Name"];
			poll.Description = data["Description"];
			poll.Image = data["Image"];

			if (data["StartTime"] != null) poll.StartTime = DateTime.Parse(data["StartTime"]);
			if (data["EndTime"] != null) poll.EndTime = DateTime.Parse(data["EndTime"]);

			if (!poll.Validate()) return BadRequest();

			context.Polls.Update(poll);
			context.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
