using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAut.Models
{
	public enum Status
	{
		Active,
		Planned,
		Over
	}
	public class Poll
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public virtual List<Vote> Votes { get; set; }
		public virtual List<Option> Options { get; set; }
		public string? Image { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public int OptionsCount { get
			{
				if(Options!=null) return Options.Count;
				return 0;
			} }
		public int VotesCount
		{
			get
			{
				if (Options != null) return Votes.Count;
				return 0;
			}
		}
		public Status Status
		{
			get
			{
				if((StartTime == null && EndTime == null)) return Status.Active;
				if (EndTime < DateTime.Now) return Status.Over;
				if (StartTime > DateTime.Now) return Status.Planned;
				if (StartTime < DateTime.Now && EndTime > DateTime.Now) return Status.Active;
				return Status.Active;
				
				
			}
		}

		public bool Validate()
		{
			if (Name.Length < 1) return false;
			if (OptionsCount < 2) return false;
			if (StartTime > EndTime) return false;
			return true;

		}
	}
}
