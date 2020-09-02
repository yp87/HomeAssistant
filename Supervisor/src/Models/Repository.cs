using System;
using System.Text.Json.Serialization;

namespace Supervisor.Models
{
    public class Repository
    {
        public long? Id { get; set; }

        [JsonPropertyName("node_id")]
        public string? NodeId { get; set; }

        public string? Name { get; set; }

        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        public bool? Private { get; set; }

        public Sender? Owner { get; set; }

        [JsonPropertyName("html_url")]
        public Uri? HtmlUrl { get; set; }

        public string? Description { get; set; }

        public bool? Fork { get; set; }

        public Uri? Url { get; set; }

        [JsonPropertyName("forks_url")]
        public Uri? ForksUrl { get; set; }

        [JsonPropertyName("keys_url")]
        public string? KeysUrl { get; set; }

        [JsonPropertyName("collaborators_url")]
        public string? CollaboratorsUrl { get; set; }

        [JsonPropertyName("teams_url")]
        public Uri? TeamsUrl { get; set; }

        [JsonPropertyName("hooks_url")]
        public Uri? HooksUrl { get; set; }

        [JsonPropertyName("issue_events_url")]
        public string? IssueEventsUrl { get; set; }

        [JsonPropertyName("events_url")]
        public Uri? EventsUrl { get; set; }

        [JsonPropertyName("assignees_url")]
        public string? AssigneesUrl { get; set; }

        [JsonPropertyName("branches_url")]
        public string? BranchesUrl { get; set; }

        [JsonPropertyName("tags_url")]
        public Uri? TagsUrl { get; set; }

        [JsonPropertyName("blobs_url")]
        public string? BlobsUrl { get; set; }

        [JsonPropertyName("git_tags_url")]
        public string? GitTagsUrl { get; set; }

        [JsonPropertyName("git_refs_url")]
        public string? GitRefsUrl { get; set; }

        [JsonPropertyName("trees_url")]
        public string? TreesUrl { get; set; }

        [JsonPropertyName("statuses_url")]
        public string? StatusesUrl { get; set; }

        [JsonPropertyName("languages_url")]
        public Uri? LanguagesUrl { get; set; }

        [JsonPropertyName("stargazers_url")]
        public Uri? StargazersUrl { get; set; }

        [JsonPropertyName("contributors_url")]
        public Uri? ContributorsUrl { get; set; }

        [JsonPropertyName("subscribers_url")]
        public Uri? SubscribersUrl { get; set; }

        [JsonPropertyName("subscription_url")]
        public Uri? SubscriptionUrl { get; set; }

        [JsonPropertyName("commits_url")]
        public string? CommitsUrl { get; set; }

        [JsonPropertyName("git_commits_url")]
        public string? GitCommitsUrl { get; set; }

        [JsonPropertyName("comments_url")]
        public string? CommentsUrl { get; set; }

        [JsonPropertyName("issue_comment_url")]
        public string? IssueCommentUrl { get; set; }

        [JsonPropertyName("contents_url")]
        public string? ContentsUrl { get; set; }

        [JsonPropertyName("compare_url")]
        public string? CompareUrl { get; set; }

        [JsonPropertyName("merges_url")]
        public Uri? MergesUrl { get; set; }

        [JsonPropertyName("archive_url")]
        public string? ArchiveUrl { get; set; }

        [JsonPropertyName("downloads_url")]
        public Uri? DownloadsUrl { get; set; }

        [JsonPropertyName("issues_url")]
        public string? IssuesUrl { get; set; }

        [JsonPropertyName("pulls_url")]
        public string? PullsUrl { get; set; }

        [JsonPropertyName("milestones_url")]
        public string? MilestonesUrl { get; set; }

        [JsonPropertyName("notifications_url")]
        public string? NotificationsUrl { get; set; }

        [JsonPropertyName("labels_url")]
        public string? LabelsUrl { get; set; }

        [JsonPropertyName("releases_url")]
        public string? ReleasesUrl { get; set; }

        [JsonPropertyName("deployments_url")]
        public Uri? DeploymentsUrl { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonPropertyName("pushed_at")]
        public DateTimeOffset? PushedAt { get; set; }

        [JsonPropertyName("git_url")]
        public string? GitUrl { get; set; }

        [JsonPropertyName("ssh_url")]
        public string? SshUrl { get; set; }

        [JsonPropertyName("clone_url")]
        public Uri? CloneUrl { get; set; }

        [JsonPropertyName("svn_url")]
        public Uri? SvnUrl { get; set; }

        public object? Homepage { get; set; }

        public long? Size { get; set; }

        [JsonPropertyName("stargazers_count")]
        public long? StargazersCount { get; set; }

        [JsonPropertyName("watchers_count")]
        public long? WatchersCount { get; set; }

        public string? Language { get; set; }

        [JsonPropertyName("has_issues")]
        public bool? HasIssues { get; set; }

        [JsonPropertyName("has_projects")]
        public bool? HasProjects { get; set; }

        [JsonPropertyName("has_downloads")]
        public bool? HasDownloads { get; set; }

        [JsonPropertyName("has_wiki")]
        public bool? HasWiki { get; set; }

        [JsonPropertyName("has_pages")]
        public bool? HasPages { get; set; }

        [JsonPropertyName("forks_count")]
        public long? ForksCount { get; set; }

        [JsonPropertyName("mirror_url")]
        public Uri? MirrorUrl { get; set; }

        public bool? Archived { get; set; }

        public bool? Disabled { get; set; }

        [JsonPropertyName("open_issues_count")]
        public long? OpenIssuesCount { get; set; }

        public object? License { get; set; }

        public long? Forks { get; set; }

        [JsonPropertyName("open_issues")]
        public long? OpenIssues { get; set; }

        public long? Watchers { get; set; }

        [JsonPropertyName("default_branch")]
        public string? DefaultBranch { get; set; }
    }
}
