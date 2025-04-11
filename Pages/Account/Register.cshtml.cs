using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrmAssistant.WebApp.Services;

namespace CrmAssistant.WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApiClient _apiClient;

        public RegisterModel(ILogger<RegisterModel> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = new();

        public class RegisterInputModel
        {
            [Required(ErrorMessage = "Full name is required")]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Username is required")]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Prepare the registration data
                var registrationData = new
                {
                    email = RegisterInput.Email,
                    username = RegisterInput.Username,
                    password = RegisterInput.Password,
                    full_name = RegisterInput.FullName
                };

                // Call the API to register the user
                await _apiClient.PostAsync<object, object>("/api/v1/users", registrationData);

                // Redirect to login page with success message
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToPage("/Account/Login");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error during registration");

                // Handle specific error cases
                if (ex.Message.Contains("400"))
                {
                    ModelState.AddModelError(string.Empty, "A user with this email or username already exists.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again later.");
                }

                return Page();
            }
        }
    }
}
