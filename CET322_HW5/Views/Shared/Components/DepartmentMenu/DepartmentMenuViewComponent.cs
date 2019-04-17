using CET322_HW5.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CET322_HW5.Views.Shared.Components.DepartmentMenu
{
	public class DepartmentMenuViewComponent:ViewComponent
	{
		private readonly SchoolContext dbContext;


		public DepartmentMenuViewComponent(SchoolContext dbContext) {
			this.dbContext = dbContext;
		}
		public async Task<IViewComponentResult> InvokeAsync() {
			var departments = await dbContext.Departments.ToListAsync();
			return View(departments);
		}
	}
}
