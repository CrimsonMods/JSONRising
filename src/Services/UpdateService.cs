using Microsoft.JSInterop;
using System.Text.Json;

namespace JSONRising.Services
{
    public class UpdateService
    {
        private readonly IJSRuntime _jsRuntime;
        private string CurrentVersion = "0.0.0";

        public UpdateService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> CheckForUpdates()
        {
            try
            {
                CurrentVersion = await _jsRuntime.InvokeAsync<string>("window.__TAURI__.core.invoke", "get_app_version");
                var metrics = await _jsRuntime.InvokeAsync<JsonElement>(
                    "window.__TAURI__.core.invoke",
                    "check_for_updates"
                );

                var latestVersion = metrics.GetProperty("latest_version").GetString();
                return CompareVersions(CurrentVersion, latestVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for updates: {ex.Message}");
                return false;
            }
        }

        private bool CompareVersions(string currentVersion, string latestVersion)
        {
            var current = Version.Parse(currentVersion);
            var latest = Version.Parse(latestVersion);
            return latest > current;
        }
    }
}