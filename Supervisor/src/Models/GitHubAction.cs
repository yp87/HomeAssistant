using System.Text.Json.Serialization;

namespace Supervisor.Models
{
    public class GitHubAction
    {
        public string? Action { get; set; }

        [JsonPropertyName("check_suite")]
        public CheckSuite? CheckSuite { get; set; }

        public Repository? Repository { get; set; }

        public Sender? Sender { get; set; }
    }
}
