using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CET322_HW5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CET322_HW5.Areas.Identity.Pages.Account.Manage
{
	public partial class IndexModel : PageModel
	{
		private readonly UserManager<SchoolUser> _userManager;
		private readonly SignInManager<SchoolUser> _signInManager;
		private readonly IEmailSender _emailSender;

		public IndexModel(
			UserManager<SchoolUser> userManager,
			SignInManager<SchoolUser> signInManager,
			IEmailSender emailSender) {
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
		}

		public string Username { get; set; }

		public bool IsEmailConfirmed { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required]
			[StringLength(100)]
			[Display(Name = "School No")]
			public string SchoolNo { get; set; }


			[Required]
			[StringLength(100)]
			[Display(Name = "First Name")]
			public string FirstName { get; set; }


			[Required]
			[StringLength(100)]
			[Display(Name = "Last Name")]
			public string LastName { get; set; }

			[Required]
			[StringLength(100)]
			[Display(Name = "City")]
			public string City { get; set; }

			[Required]
			[EmailAddress]
			public string Email { get; set; }

			[Phone]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }
		}

		public async Task<IActionResult> OnGetAsync() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) {
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var userName = await _userManager.GetUserNameAsync(user);
			var email = await _userManager.GetEmailAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			var SignedUser = _userManager.Users.Where(x => x.UserName == userName).FirstOrDefault();

			Username = userName;

			Input = new InputModel {
				Email = email,
				PhoneNumber = phoneNumber,
				City = SignedUser.City,
				FirstName = SignedUser.FirstName,
				LastName = SignedUser.LastName,
				SchoolNo = SignedUser.SchoolNumber
			};

			IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

			return Page();
		}

		public async Task<IActionResult> OnPostAsync() {
			if (!ModelState.IsValid) {
				return Page();
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null) {
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var email = await _userManager.GetEmailAsync(user);
			if (Input.Email != email) {
				var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
				if (!setEmailResult.Succeeded) {
					var userId = await _userManager.GetUserIdAsync(user);
					throw new InvalidOperationException($"Unexpected error occurred setting email for user with ID '{userId}'.");
				}
			}
			 _userManager.Users.Where(x => x.Email == email).FirstOrDefault().City = Input.City;
			_userManager.Users.Where(x => x.Email == email).FirstOrDefault().SchoolNumber = Input.SchoolNo;
			_userManager.Users.Where(x => x.Email == email).FirstOrDefault().FirstName = Input.FirstName;
			_userManager.Users.Where(x => x.Email == email).FirstOrDefault().LastName = Input.LastName;
			
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			if (Input.PhoneNumber != phoneNumber) {
				var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
				if (!setPhoneResult.Succeeded) {
					var userId = await _userManager.GetUserIdAsync(user);
					throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
				}
			}

			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostSendVerificationEmailAsync() {
			if (!ModelState.IsValid) {
				return Page();
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null) {
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}


			var userId = await _userManager.GetUserIdAsync(user);
			var email = await _userManager.GetEmailAsync(user);
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				pageHandler: null,
				values: new { userId = userId, code = code },
				protocol: Request.Scheme);
			await _emailSender.SendEmailAsync(
				email,
				"Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

			StatusMessage = "Verification email sent. Please check your email.";
			return RedirectToPage();
		}
	}
}
