using System;
using Supervisor.Automation;
using Supervisor.Models;

namespace Supervisor.ActionHandlers
{
    public class CheckRunActionHandler : IActionHandler
    {
        private readonly IAutomationUpdater _automationUpdater;

        public CheckRunActionHandler(IAutomationUpdater automationUpdater)
        {
            _automationUpdater = automationUpdater;
        }

        public bool CanHandleAction(string actionName) =>
            actionName.Equals("check_run", StringComparison.InvariantCultureIgnoreCase);

        public void Handle(GitHubAction action)
        {
            if ((action.CheckRun?.Status?.Equals("completed", StringComparison.InvariantCultureIgnoreCase) ?? false) &&
                (action.CheckRun.Conclusion?.Equals("success", StringComparison.InvariantCultureIgnoreCase)?? false) &&
                (action.CheckRun.CheckSuite?.HeadBranch?.Equals("master", StringComparison.InvariantCultureIgnoreCase) ?? false) &&
                (action.CheckRun.CheckSuite.PullRequests?.Length ?? 0) == 0)
            {
                _automationUpdater.Update();
            }
        }
    }
}
