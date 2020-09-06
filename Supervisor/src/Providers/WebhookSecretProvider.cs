namespace Supervisor.Providers
{
    public class WebhookSecretProvider
    {
        public string WebhookSecret { get; }

        public WebhookSecretProvider(string webhookSecret)
        {
            WebhookSecret = webhookSecret;
        }
    }
}
