namespace Supervisor.Providers
{
    public class AutomationEndpointProvider
    {
        public string AutomationEndpoint { get; }

        public AutomationEndpointProvider(string automationEndpoint)
        {
            AutomationEndpoint = automationEndpoint;
        }
    }
}
