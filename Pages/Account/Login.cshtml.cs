using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrmAssistant.WebApp.Services;

namespace CrmAssistant.WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly ApiClient _apiClient;

        public LoginModel(ILogger<LoginModel> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        [BindProperty]
        public LoginInputModel LoginInput { get; set; } = new();

        public class LoginInputModel
        {
            [Required(ErrorMessage = "Username is required")]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
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
                // Prepare the login data
                var loginData = new
                {
                    username = LoginInput.Username,
                    password = LoginInput.Password
                };

                // Call the API to login
                var response = await _apiClient.PostAsync<object, JsonElement>("/api/v1/auth/login/json", loginData);

                // Extract the token from the response
                string token = response.GetProperty("access_token").GetString() ?? string.Empty;
                string tokenType = response.GetProperty("token_type").GetString() ?? "bearer";

                if (string.IsNullOrEmpty(token))
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                // Store the token in a cookie or session (this is a simplified approach)
                // In a real application, you would use a more secure approach
                HttpContext.Response.Cookies.Append(
                    "AuthToken",
                    token,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = Request.IsHttps,
                        SameSite = SameSiteMode.Lax,
                        Expires = LoginInput.RememberMe ? 
                            DateTimeOffset.UtcNow.AddDays(7) : 
                            DateTimeOffset.UtcNow.AddHours(1)
                    }
                );

                // Redirect to home page or dashboard
                return RedirectToPage("/Index");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error during login");

                // Handle specific error cases
                if (ex.Message.Contains("400"))
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again later.");
                }

                return Page();
            }
        }
    }
}
