using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CrmAssistant.WebApp.Services;

namespace CrmAssistant.WebApp.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly ApiClient _apiClient;

        public LogoutModel(ILogger<LogoutModel> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Call the API to logout (if implemented)
                // await _apiClient.PostAsync("/api/v1/auth/logout", null);
                
                // Remove the auth token cookie
                if (HttpContext.Request.Cookies.ContainsKey("AuthToken"))
                {
                    HttpContext.Response.Cookies.Delete("AuthToken");
                }
                
                // Redirect to home page
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                // Even if there's an error, we still want to remove the cookie and redirect
                HttpContext.Response.Cookies.Delete("AuthToken");
                return RedirectToPage("/Index");
            }
        }
    }
}
