namespace Supervisor.Models
{
    public class GitHubAction
    {
        public string Action { get; set; }

        public CheckRun CheckRun { get; set; }

        public Repository Repository { get; set; }

        public Sender Sender { get; set; }
    }
}
