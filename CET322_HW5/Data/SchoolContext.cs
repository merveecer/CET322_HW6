using System;
using System.Collections.Generic;
using System.Text;
using CET322_HW5.Data;
using CET322_HW5.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CET322_HW5.Data
{
	public class SchoolContext : IdentityDbContext<SchoolUser>
	{
		public SchoolContext(DbContextOptions<SchoolContext> options)
			: base(options) {
		}
		public DbSet<Department> Departments { get; set; }
		public DbSet<Student> Students { get; set; }
	}
}
