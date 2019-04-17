using CET322_HW5.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CET322_HW5.Models
{
    public class DepartmentModel
    {
        public int Id { get; set; }
        [Display(Name = "Department Name: ")]
        public string Name { get; set; }
		public string SelectedDepartmentManagerId { get; set; }

		public virtual IEnumerable<SelectListItem> AvailableManagers { get; set; }
        public virtual IList<Student> Students { get; set; }
	}
}
