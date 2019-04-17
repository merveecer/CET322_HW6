using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CET322_HW5.Data;
using CET322_HW5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CET322_HW5.Controllers
{
	[Authorize(Roles = "admin,departmenManager")]

	public class DepartmentsController : Controller
	{
		private readonly SchoolContext _context;
		private readonly UserManager<SchoolUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		#region Ctor
		public DepartmentsController(SchoolContext context, UserManager<SchoolUser> userManager, RoleManager<IdentityRole> roleManager) {
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}
		#endregion

		#region Methods
		public IList<Student> GetStudentsByDepartmentId(int id) {
			IList<Student> students;
			students = _context.Students.Where(x => x.DepartmentId == id).ToList();
			return students;

		}


		private IList<SelectListItem> GetAvailableManagers(IList<SchoolUser> managers) {
			var availableManagers = new List<SelectListItem>();
			foreach (var manager in managers) {
				availableManagers.Add(new SelectListItem {
					Value = manager.Id.ToString(),
					Text = manager.FirstName + " " + manager.LastName
				});

			}
			availableManagers.Insert(0, new SelectListItem {
				Value = "0",
				Text = "Please select a manager"
			});
			return availableManagers;
		}

		#endregion
		#region Details
		[AllowAnonymous]
		public IActionResult Detail(int id) {
			var department = _context.Departments.Where(x => x.Id == id).FirstOrDefault();
			if (department != null) {
				DepartmentModel departmentModel = new DepartmentModel();
				departmentModel.Name = department.Name;
				departmentModel.Students = GetStudentsByDepartmentId(id);
				departmentModel.Id = department.Id;
				return View(departmentModel);

			} else
				return NotFound();

		}
		#endregion

		#region List	
		[AllowAnonymous]
		public IActionResult DepartmentList() {
			var departments = _context.Departments.ToList();
			var departmentsModel = new List<DepartmentModel>();
			foreach (var item in departments) {
				var model = new DepartmentModel {
					Id = item.Id,
					Name = item.Name
				};

				departmentsModel.Add(model);
			}

			return View(departmentsModel);
		}
		#endregion

		#region Create
		public IActionResult Create() {
			DepartmentModel departmentModel = new DepartmentModel();

			var userList = _context.Users.ToList();
			departmentModel.AvailableManagers = GetAvailableManagers(userList);
			return View(departmentModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(DepartmentModel model) {
			if (ModelState.IsValid) {
				var existingDepartment = _context.Departments.Where(x => x.Name == model.Name).FirstOrDefault();
				if (existingDepartment == null) {
					Department newdepartment = new Department {
						Name = model.Name,
						DepartmentAdminId = model.SelectedDepartmentManagerId

					};

					_context.Departments.Add(newdepartment);
					if (!(await _roleManager.RoleExistsAsync("departmentManager"))) {
						await _roleManager.CreateAsync(new IdentityRole { Name = "departmentManager" });

					}
					var user = await _userManager.FindByIdAsync(model.SelectedDepartmentManagerId.ToString());

					await _userManager.AddToRoleAsync(user, "departmentManager");
					_context.SaveChanges();
				}
				return RedirectToAction("DepartmentList");
			} else
				return View(model);
		}
		#endregion

		#region Edit
		public IActionResult Edit(int? id) {
			if (!id.HasValue) {
				return BadRequest();
			}
			var department = _context.Departments.Where(x => x.Id == id).FirstOrDefault();
			if (department == null) {
				return NotFound();
			}
			var userList = _context.Users.ToList();

			var model = new DepartmentModel {
				Id = department.Id,
				Name = department.Name,
				AvailableManagers = GetAvailableManagers(userList),
				SelectedDepartmentManagerId = department.DepartmentAdminId


			};
			return View(model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(int? id, DepartmentModel model) {
			var department = _context.Departments.Where(x => x.Id == model.Id).FirstOrDefault();

			if (!id.HasValue) {
				return BadRequest();
			}

			if (department == null) {
				return NotFound();
			}

			if (id != department.Id) {
				return BadRequest();
			}
			if (ModelState.IsValid && department != null) {


				department.Name = model.Name;
				department.DepartmentAdminId = model.SelectedDepartmentManagerId;
				if (!(await _roleManager.RoleExistsAsync("departmentManager"))) {
					await _roleManager.CreateAsync(new IdentityRole { Name = "departmentManager" });

				}
				var user = await _userManager.FindByIdAsync(model.SelectedDepartmentManagerId.ToString());

				await _userManager.AddToRoleAsync(user, "departmentManager");
				_context.Departments.Update(department);
				_context.SaveChanges();
				return RedirectToAction("Edit");
			} else {
				return View(department);

			}


		}
		#endregion

		#region Delete
		public IActionResult Delete(int id) {
			var department = _context.Departments.Where(x => x.Id == id).FirstOrDefault();
			if (department != null) {
				_context.Departments.Remove(department);
				_context.SaveChanges();

				return RedirectToAction("DepartmentList");
			}
			return NotFound();
		}

		#endregion
	}
}