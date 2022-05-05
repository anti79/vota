using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyAut.Models
{
	public class Vote
	{
		public int Id { get; set; }	
		public string UserId { get; set; }
		
		public int OptionId { get; set; }
	}
}
