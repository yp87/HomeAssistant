using System.Text.Json.Serialization;

namespace Supervisor.Models
{
    public class GitHubAction
    {
        public string? Action { get; set; }

        [JsonPropertyName("check_run")]
        public CheckRun? CheckRun { get; set; }

        public Repository? Repository { get; set; }

        public Sender? Sender { get; set; }
    }
}
