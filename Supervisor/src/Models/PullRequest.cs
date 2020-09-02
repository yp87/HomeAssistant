using System;

namespace Supervisor.Models
    {
        public class PullRequest
        {
            public Uri? Url { get; set; }

            public long? Id { get; set; }

            public long? Number { get; set; }

            public Base? Head { get; set; }

            public Base? Base { get; set; }
        }
    }
