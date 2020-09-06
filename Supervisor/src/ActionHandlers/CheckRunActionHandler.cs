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

        private readonly IAutomationClient _automationClient;

        private readonly SemaphoreSlim _automationSemaphore = new SemaphoreSlim(1);

        public CheckRunActionHandler(IAutomationUpdater automationUpdater, IAutomationClient automationClient)
        {
            _automationUpdater = automationUpdater;
            _automationClient = automationClient;
        }

        public bool CanHandleAction(string actionName) =>
            actionName.Equals("check_run", StringComparison.InvariantCultureIgnoreCase);

        public async Task HandleAsync(GitHubAction action)
        {
            await _automationSemaphore.WaitAsync();
            try
            {
                await _automationClient.NotifyAsync($"A build was {action.CheckRun?.Status}.");
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
