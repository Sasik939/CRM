using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using CrmAssistant.WebApp.Services;

namespace CrmAssistant.WebApp.Pages
{
    public class HealthModel : PageModel
    {
        private readonly ILogger<HealthModel> _logger;
        private readonly ApiClient _apiClient;

        public HealthModel(ILogger<HealthModel> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public bool IsApiHealthy { get; private set; }
        public bool IsDatabaseHealthy { get; private set; }
        public string ApiVersion { get; private set; } = string.Empty;
        public string ErrorMessage { get; private set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                // Call the API health check endpoint
                var healthResult = await _apiClient.GetAsync<JsonElement>("/api/v1/health");
                
                // Parse the response
                IsApiHealthy = healthResult.GetProperty("status").GetString() == "ok";
                
                if (healthResult.TryGetProperty("api_version", out var versionElement))
                {
                    ApiVersion = versionElement.GetString() ?? string.Empty;
                }
                
                if (healthResult.TryGetProperty("database", out var dbElement))
                {
                    IsDatabaseHealthy = dbElement.GetString() == "ok";
                }
            }
            catch (Exception ex)
            {
                IsApiHealthy = false;
                IsDatabaseHealthy = false;
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error checking API health");
            }
        }
    }
}
