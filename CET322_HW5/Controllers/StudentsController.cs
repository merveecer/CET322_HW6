using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CET322_HW5.Data;
using CET322_HW5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CET322_HW5.Controllers
{
	[Authorize(Roles = "admin,departmentManager")]
	public class StudentsController : Controller
	{
		private readonly SchoolContext _context;
		private readonly IHostingEnvironment _hostingEnvironment;
		private readonly UserManager<SchoolUser> _userManager;

		#region Ctor
		public StudentsController(SchoolContext context, IHostingEnvironment hostingEnvironment, UserManager<SchoolUser> userManager) {
			_context = context;
			_hostingEnvironment = hostingEnvironment;
			_userManager = userManager;
		}
		#endregion

		#region Methods
		private IList<SelectListItem> GetAvailableDepartments(IList<Department> departments) {
			var availableDepartments = new List<SelectListItem>();
			foreach (var department in departments) {
				availableDepartments.Add(new SelectListItem {
					Value = department.Id.ToString(),
					Text = department.Name
				});

			}
			availableDepartments.Insert(0, new SelectListItem {
				Value = "0",
				Text = "Please select a department"
			});
			return availableDepartments;
		}
		#endregion

		[AllowAnonymous]
		#region List
		public IActionResult StudentList() {
			var students = _context.Students.Include(x => x.Department).Include(y=>y.SchoolUser).ToList();
			var studentsmodel = new List<StudentModel>();
			foreach (var item in students) {
				var model = new StudentModel {
					Id = item.Id,
					Name = item.Name,
					Email = item.Email,
					SchoolNumber = item.SchoolNumber,
					Surname = item.Surname,
					SelectedDepartmentId = item.DepartmentId,
					Department = item.Department,
					CreatedDate = item.CreatedDate,
					ImageUrl = item.ImageUrl,
					PersonalInfo = item.PersonalInfo,
					SchoolUser = item.SchoolUser
					

			};
				model.UserofSelectedDepartment = _context.Users.Where(x => x.Id == item.Department.DepartmentAdminId).FirstOrDefault();
				studentsmodel.Add(model);
			}

			return View(studentsmodel);
		}
		#endregion

		[AllowAnonymous]
		#region Detail
		public IActionResult Detail(int id) {
			var student = _context.Students.Include(x => x.Department).Include(x => x.SchoolUser).Where(x => x.Id == id).FirstOrDefault();
			if (student != null) {
				StudentModel studentModel = new StudentModel();
				studentModel.Name = student.Name;
				studentModel.Surname = student.Surname;
				studentModel.Email = student.Email;
				studentModel.SchoolNumber = student.SchoolNumber;
				studentModel.SelectedDepartmentId = student.DepartmentId;
				studentModel.Department = student.Department;
				studentModel.ImageUrl = student.ImageUrl;
				studentModel.PersonalInfo = student.PersonalInfo;
				studentModel.CreatedDate = student.CreatedDate;
				studentModel.CreaterFullName=student.SchoolUser.FirstName+" "+student.SchoolUser.LastName;
				return View(studentModel);

			} else
				return NotFound();

		}
		#endregion

		#region Create
		public IActionResult Create() {
			StudentModel studentModel = new StudentModel();
			var departments = _context.Departments.OrderBy(x => x.Name).ToList();
			studentModel.AvailableDepartments = GetAvailableDepartments(departments);
			return View(studentModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
	
		public IActionResult Create(StudentModel model) {
			var loginUserId = _userManager.GetUserId(User);
			var existingStudent = _context.Students.Where(x => x.SchoolNumber == model.SchoolNumber).FirstOrDefault();
			var department = _context.Departments.Where(x => x.Id == model.SelectedDepartmentId).FirstOrDefault();
			if (ModelState.IsValid && model.SelectedDepartmentId != 0) {
				if (existingStudent == null) {

					var selectedDepartment = _context.Departments.Where(y => y.Id == model.SelectedDepartmentId).FirstOrDefault();
					var userOfselectedDepartment = _context.Users.Where(x => x.Id == selectedDepartment.DepartmentAdminId).FirstOrDefault();
					if ((User.IsInRole("departmentManager") && !(User.Identity.Name == userOfselectedDepartment.UserName) && !(User.IsInRole("admin")))) {
						return Unauthorized();
					}
					Student newstudent = new Student {
						Email = model.Email,
						Name = model.Name,
						SchoolNumber = model.SchoolNumber,
						Surname = model.Surname,
						DepartmentId = model.SelectedDepartmentId,
						Department = department,
						CreatedDate = DateTime.Now,
						PersonalInfo = model.PersonalInfo,
						SchoolUserId = loginUserId,
						SchoolUser = _userManager.Users.Where(x => x.Id == loginUserId).FirstOrDefault()
						 
					};
					

					string dirPath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads\");
					if (model.ImageFile != null) {
						var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + model.ImageFile.FileName;
						using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create)) {
							model.ImageFile.CopyTo(fileStream);
						}
						newstudent.ImageUrl = fileName;
					}
					_context.Students.Add(newstudent);
					_context.SaveChanges();

				}
				return RedirectToAction("StudentList");
			} else {
				return View(model);
			}
		}
		#endregion

		#region Edit
		public IActionResult Edit(int? id) {
			var student = _context.Students.Where(x => x.Id == id).FirstOrDefault();

			if (!id.HasValue) {
				return BadRequest();
			}

			if (student == null) {
				return NotFound();
			}
			var departments = _context.Departments.OrderBy(x => x.Name).ToList();
			var model = new StudentModel {
				Id = student.Id,
				Email = student.Email,
				Name = student.Name,
				SchoolNumber = student.SchoolNumber,
				Surname = student.Surname,
				SelectedDepartmentId = student.DepartmentId,
				AvailableDepartments = GetAvailableDepartments(departments),
				ImageUrl = student.ImageUrl,
				PersonalInfo = student.PersonalInfo
				 
			};



			return View(model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(int? id, StudentModel model) {
			if (!id.HasValue) {
				return BadRequest();
			}

			if (model == null) {
				return NotFound();
			}
			var student = _context.Students.Where(x => x.Id == model.Id).FirstOrDefault();
			if (id != student.Id)
				return BadRequest();
			if (ModelState.IsValid && student != null && model.SelectedDepartmentId != 0) {
				var currentStudent = _context.Students.Include(s => s.SchoolUser).FirstOrDefault(s => s.Id == model.Id);
				//var currentStudent = _context.Students.Include(p =>p.SchoolUser ).FirstOrDefaultAsync(s => s.Id == post.Id);
				if (!(currentStudent.SchoolUser?.UserName == User.Identity.Name || User.IsInRole("admin") || User.IsInRole("departmentManager"))) {
					return Unauthorized();

				}
				var selectedDepartment = _context.Departments.Where(y => y.Id == model.SelectedDepartmentId).FirstOrDefault();
				var userOfselectedDepartment = _context.Users.Where(x => x.Id == selectedDepartment.DepartmentAdminId).FirstOrDefault();
				if((User.IsInRole("departmentManager") && !(User.Identity.Name == userOfselectedDepartment.UserName) && !(User.IsInRole("admin")))){
					return Unauthorized();
				}
				var loginUserId = _userManager.GetUserId(User);
				student.Name = model.Name;
				student.Surname = model.Surname;
				student.SchoolNumber = model.SchoolNumber;
				student.Email = model.Email;
				student.DepartmentId = model.SelectedDepartmentId;
				student.Department = _context.Departments.Where(x => x.Id == model.SelectedDepartmentId).FirstOrDefault();
				student.PersonalInfo = model.PersonalInfo;
				if (model.ImageFile != null) {
					string dirPath = Path.Combine(_hostingEnvironment.WebRootPath, @"uploads\");
					var fileName = Guid.NewGuid().ToString().Replace("-", "") + "_" + model.ImageFile.FileName;
					using (var fileStream = new FileStream(dirPath + fileName, FileMode.Create)) {
						model.ImageFile.CopyTo(fileStream);
					}
					student.ImageUrl = fileName;

				}
				student.SchoolUserId = loginUserId;
				student.SchoolUser = _userManager.Users.Where(x => x.Id == loginUserId).FirstOrDefault();
				_context.Students.Update(student);
				_context.SaveChanges();
				return RedirectToAction("Edit");
			} else {
				return View(student);

			}


		}
		#endregion

		#region Delete
		public IActionResult Delete(int id) {
			var student = _context.Students.Where(x => x.Id == id).FirstOrDefault();
			var selectedDepartment = _context.Departments.Where(y => y.Id == student.DepartmentId).FirstOrDefault();
			var userOfselectedDepartment = _context.Users.Where(x => x.Id == selectedDepartment.DepartmentAdminId).FirstOrDefault();
			if ((User.IsInRole("departmentManager") && !(User.Identity.Name == userOfselectedDepartment.UserName))) {
				return Unauthorized();
			}
			if (student != null) {
				_context.Students.Remove(student);
				_context.SaveChanges();

				return RedirectToAction("StudentList");
			}
			return NotFound();
		}

		#endregion


	}

}
