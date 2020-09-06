namespace Supervisor.Providers
{
    public class AutomationApiKeyProvider
    {
        public string AutomationApiKey { get; }

        public AutomationApiKeyProvider(string automationApiKey)
        {
            AutomationApiKey = automationApiKey;
        }
    }
}
