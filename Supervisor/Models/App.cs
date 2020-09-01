using System;
using System.Text.Json.Serialization;

namespace Supervisor.Models
{
    public class App
    {
        public long Id { get; set; }

        public string Slug { get; set; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        public Sender Owner { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [JsonPropertyName("external_url")]
        public Uri ExternalUrl { get; set; }

        [JsonPropertyName("html_url")]
        public Uri HtmlUrl { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        public Permissions Permissions { get; set; }

        public string[] Events { get; set; }
    }
}
