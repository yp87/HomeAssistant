using System;
using Supervisor.Automation;
using Supervisor.Models;
using System.Threading.Tasks;
using System.Threading;

namespace Supervisor.ActionHandlers
{
    public class CheckRunActionHandler : IActionHandler
    {
        private readonly IAutomationUpdater _automationUpdater;

        private readonly SemaphoreSlim _automationSemaphore = new SemaphoreSlim(1);

        public CheckRunActionHandler(IAutomationUpdater automationUpdater)
        {
            _automationUpdater = automationUpdater;
        }

        public bool CanHandleAction(string actionName) =>
            actionName.Equals("check_run", StringComparison.InvariantCultureIgnoreCase);

        public async Task HandleAsync(GitHubAction action)
        {
            await _automationSemaphore.WaitAsync();
            try
            {
                if ((action.CheckRun?.Status?.Equals(Constants.GitHubBuildCompleted, StringComparison.InvariantCultureIgnoreCase) ?? false) &&
                    (action.CheckRun.Conclusion?.Equals(Constants.GitHubBuildSuccess, StringComparison.InvariantCultureIgnoreCase)?? false) &&
                    (action.CheckRun.CheckSuite?.HeadBranch?.Equals(Constants.MasterBranchName, StringComparison.InvariantCultureIgnoreCase) ?? false) &&
                    (action.CheckRun.CheckSuite.PullRequests?.Length ?? 0) == 0)
                {
                    await _automationUpdater.UpdateAsync();
                }
            }
            finally
            {
                _automationSemaphore.Release();
            }

        }
    }
}
