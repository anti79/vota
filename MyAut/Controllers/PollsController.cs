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
			ViewBag.Status = Status.Over;
			return View("Old");
		}
		public IActionResult Future()
		{

			var list = Context.GetPlanned().ToList();
			ViewBag.Polls = list;
			return View("Future");
		}
		[HttpGet("Polls/Vote/{id}")]
		public IActionResult Vote(int id)
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
			ViewBag.Status = poll.Status;
			foreach(Option option in poll.Options)
			{
				counts[option.Id] = poll.Votes.Where(v => v.OptionId == option.Id).Count();
			}
			ViewBag.Total = total;
			ViewBag.Counts = counts;
			ViewBag.Percentages = percentages;
			if (poll.Status == Status.Over) {
				var max = counts.FirstOrDefault(x => x.Value.Equals(counts.Values.Max()));
				if(counts.Where(x => x.Value.Equals(max.Value)).Count()==1) ViewBag.WonId = max.Key;
					}
			foreach(var pair in counts)
			{
				percentages[pair.Key] = Math.Round((double)((double)(pair.Value) / total) * 100);
				if (total == 0) percentages[pair.Key] = 0;
			}

			return View("Vote",Context.Polls.Where(p=>p.Id==id).FirstOrDefault());
		}
		[HttpPost]
		public IActionResult CastVote(int optionId, int pollId)
		{
			string userId = userManager.GetUserId(User);
			var poll = Context.Polls.Where(p => p.Id == pollId).FirstOrDefault();
			if (Context.UserHasVoted(userId,pollId) || poll.Status!=Status.Active)
			{
				return BadRequest();
			}
			Vote vote = new Vote() { OptionId = optionId, UserId =  userId};
			
			poll.Votes.Add(vote);
			Context.SaveChanges();
			return View("Success");
		}
	}
}
