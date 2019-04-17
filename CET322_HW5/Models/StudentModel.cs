using CET322_HW5.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CET322_HW5.Models
{
    public class StudentModel
    {
        public int Id { get; set; }

        [Display(Name = "Name: ")]
        [Required(ErrorMessage = "Student Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The student name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Student Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The student surname must be between 2 and 100 characters")]
        [Display(Name = "Surname: ")]
        public string Surname { get; set; }

        [Display(Name = "School Number: ")]
        public string SchoolNumber { get; set; }

        [Display(Name = "Email: ")]
        public string Email { get; set; }

		[Required]
		[Display(Name = "Personal Info: ")]
		public string PersonalInfo { get; set; }

		public string ImageUrl { get; set; }

		[Display(Name = "Created Date: ")]
		public DateTime CreatedDate { get; set; }
		[Display(Name = "DEpartment Name: ")]
		public string DepartmentName { get; set; }

		public virtual IEnumerable<SelectListItem> AvailableDepartments { get; set; }
        
        public int SelectedDepartmentId { get; set; }

        public Department Department { get; set; }


		public IFormFile ImageFile { get; set; }
		[Display(Name = "Creater: ")]
		public string CreaterFullName { get; set; }

		public SchoolUser SchoolUser { get; set; }
		public SchoolUser UserofSelectedDepartment { get; set; }
	}
}
