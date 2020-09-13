using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Supervisor.Models;
using Supervisor.Providers;

namespace Supervisor.Automation
{
    public class HomeAssistantClient : IAutomationClient, IDisposable
    {
        private readonly HttpClient _homeAssistantHttpClient;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        private readonly string _homeAssistantEndpoint;

        public HomeAssistantClient(AutomationEndpointProvider homeAssistantEndpointProvider, AutomationApiKeyProvider bearerTokenProvider)
        {
            _homeAssistantHttpClient = new HttpClient();
            _homeAssistantHttpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearerTokenProvider.AutomationApiKey);

            _homeAssistantEndpoint = homeAssistantEndpointProvider.AutomationEndpoint;

            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        public void Dispose()
        {
            _homeAssistantHttpClient.Dispose();
        }

        public Task RestartAutomationAsync()
        {
           return CallServiceAsync("homeassistant", "restart");
        }

        public async Task NotifyAsync(string notification)
        {
            var message = new HomeAssistionNotificationData()
            {
                Message = notification,
            };

            await CallServiceAsync("notify", "yan", message);
        }

        private async Task CallServiceAsync(string domain, string service, object? data = null)
        {
            StringContent? content = null;
            if (data != null)
            {
                var payload = JsonSerializer.Serialize(data, _jsonSerializerOptions);
                content = new StringContent(payload, Encoding.UTF8, "application/json");
            }

            await _homeAssistantHttpClient.PostAsync(
                $"https://{_homeAssistantEndpoint}/api/services/{domain}/{service}", content);
        }
    }
}
