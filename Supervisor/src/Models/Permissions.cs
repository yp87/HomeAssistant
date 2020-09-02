using System.Text.Json.Serialization;

namespace Supervisor.Models
{
    public class Permissions
    {
        public string? Checks { get; set; }

        public string? Contents { get; set; }

        public string? Deployments { get; set; }

        public string? Members { get; set; }

        public string? Metadata { get; set; }

        [JsonPropertyName("pull_requests")]
        public string? PullRequests { get; set; }

        [JsonPropertyName("repository_hooks")]
        public string? RepositoryHooks { get; set; }

        public string? Statuses { get; set; }
    }
}
