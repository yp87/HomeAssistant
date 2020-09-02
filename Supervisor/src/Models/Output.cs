using System;
using System.Text.Json.Serialization;

namespace Supervisor.Models
{
    public class Output
    {
        public string? Title { get; set; }

        public string? Summary { get; set; }

        public string? Text { get; set; }

        [JsonPropertyName("annotations_count")]
        public long? AnnotationsCount { get; set; }

        [JsonPropertyName("annotations_url")]
        public Uri? AnnotationsUrl { get; set; }
    }
}
