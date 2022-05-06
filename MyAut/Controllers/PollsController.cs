using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
		UserManager<User> userManager;
		public PollsController(ApplicationContext context, UserManager<User> userManager)
		{
			Context = context;
			this.userManager = userManager;
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
			string userId = userManager.GetUserId(User);
			var poll = Context.Polls.Where(p => p.Id == id).FirstOrDefault();
			ViewBag.Voted = Context.UserHasVoted(userId, id);
			if(ViewBag.Voted)
			{
				ViewBag.VotedId = poll.Votes.Where(v => userId == v.UserId).FirstOrDefault().OptionId;
			}

			Dictionary<int, int> counts = new(); //move to db logic
			Dictionary<int, double> percentages = new();  //optionId - value
			int total = poll.Votes.Count();

			foreach(Option option in poll.Options)
			{
				counts[option.Id] = poll.Votes.Where(v => v.OptionId == option.Id).Count();
			}
			ViewBag.Total = total;
			ViewBag.Counts = counts;
			ViewBag.Percentages = percentages;
			foreach(var pair in counts)
			{
				percentages[pair.Key] = (double)((double)(pair.Value) / total) * 100;
			}

			return View("Vote",Context.Polls.Where(p=>p.Id==id).FirstOrDefault());
		}
		[HttpPost]
		public IActionResult CastVote(int optionId, int pollId)
		{
			string userId = userManager.GetUserId(User);
			if (Context.UserHasVoted(userId,pollId))
			{
				return View("Error");
			}
			Vote vote = new Vote() { OptionId = optionId, UserId =  userId};
			var poll = Context.Polls.Where(p => p.Id == pollId).FirstOrDefault();
			poll.Votes.Add(vote);
			Context.SaveChanges();
			return View("Success");
		}
	}
}
