using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BlogProj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogProj.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace BlogProj.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly SignInManager<BlogUser> _signInManager;
        private readonly IBlogFileService _fileService;
        private readonly IConfiguration _configuration;

        public IndexModel(
            UserManager<BlogUser> userManager,
            SignInManager<BlogUser> signInManager,
            IBlogFileService fileService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileService = fileService;
            _configuration = configuration;
        }

        public string Username { get; set; }

        public string CurrentImage { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Custom Image")]
            public IFormFile ImageData { get; set; }

            public byte[] ProfilePic { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            [Display(Name = "Display Name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]

            public string DisplayName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Display(Name = "New Image")]
            public IFormFile NewImage { get; set; }
        }

        private async Task LoadAsync(BlogUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            CurrentImage = _fileService.DecodeImage(user.ImageData, user.ContentType);

            Input = new InputModel
            {
                DisplayName = user.DisplayName,
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if(user.DisplayName != Input.DisplayName){
                user.DisplayName = Input.DisplayName;
                await _userManager.UpdateAsync(user);
            }

            if(Input.NewImage is not null)
            {
                user.ImageData =await _fileService.EncodeFileAsync(Input.NewImage);
                user.ContentType = _fileService.ContentType(Input.NewImage);

                await _userManager.UpdateAsync(user);
            }
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
