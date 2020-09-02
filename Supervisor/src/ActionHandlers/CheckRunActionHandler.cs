using System;
using Supervisor.Models;

namespace Supervisor.ActionHandlers
{
    public class CheckRunActionHandler : IActionHandler
    {
        public bool CanHandleAction(string actionName) =>
            actionName.Equals("check_run", StringComparison.InvariantCultureIgnoreCase);

        public void Handle(GitHubAction action)
        {
            throw new NotImplementedException();
        }
    }
}
