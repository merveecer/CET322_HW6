using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CET322_HW5.Models
{
	public class SchoolUser : IdentityUser
	{
		[Required]
		[StringLength(100)]
		public string SchoolNumber { get; set; }

		[Required]
		[StringLength(100)]
		public string FirstName { get; set; }


		[Required]
		[StringLength(100)]
		public string LastName { get; set; }
		[Required]
		[StringLength(100)]
		public string City { get; set; }
	}
}
