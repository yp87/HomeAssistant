using System;
using System.Text.Json.Serialization;

namespace Supervisor.Models
{
    public class CheckRun
    {
        public long? Id { get; set; }

        [JsonPropertyName("node_id")]
        public string? NodeId { get; set; }

        [JsonPropertyName("head_sha")]
        public string? HeadSha { get; set; }

        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        public Uri? Url { get; set; }

        [JsonPropertyName("html_url")]
        public Uri? HtmlUrl { get; set; }

        [JsonPropertyName("details_url")]
        public Uri? DetailsUrl { get; set; }

        public string? Status { get; set; }

        public string? Conclusion { get; set; }

        [JsonPropertyName("started_at")]
        public DateTimeOffset? StartedAt { get; set; }

        [JsonPropertyName("completed_at")]
        public DateTimeOffset? CompletedAt { get; set; }

        public Output? Output { get; set; }

        public string? Name { get; set; }

        [JsonPropertyName("check_suite")]
        public CheckSuite? CheckSuite { get; set; }

        public App? App { get; set; }

        [JsonPropertyName("pull_requests")]
        public PullRequest?[]? PullRequests { get; set; }
    }
}
