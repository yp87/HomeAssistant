namespace Supervisor.Models
{
    public partial class Base
    {
        public string? Ref { get; set; }

        public string? Sha { get; set; }

        public Repository? Repo { get; set; }
    }
}
