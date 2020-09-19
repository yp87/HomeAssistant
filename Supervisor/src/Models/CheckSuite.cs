using System;
using System.Text.Json.Serialization;

namespace Supervisor.Models
{
    public class CheckSuite
    {
        public long? Id { get; set; }

        [JsonPropertyName("node_id")]
        public string? NodeId { get; set; }

        [JsonPropertyName("head_branch")]
        public string? HeadBranch { get; set; }

        [JsonPropertyName("head_sha")]
        public string? HeadSha { get; set; }

        public string? Status { get; set; }

        public string? Conclusion { get; set; }

        public Uri? Url { get; set; }

        public string? Before { get; set; }

        public string? After { get; set; }

        [JsonPropertyName("pull_requests")]
        public PullRequest?[]? PullRequests { get; set; }

        public App? App { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
