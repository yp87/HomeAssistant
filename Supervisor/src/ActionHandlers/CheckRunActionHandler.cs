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
                string buildStatus = action.CheckRun?.Status ?? string.Empty;
                string buildBranch = action.CheckRun?.CheckSuite?.HeadBranch ?? string.Empty;
                var buildEventNotification = $"A build was {action.CheckRun?.Status} on branch {buildBranch}.";
                if (buildStatus.Equals(Constants.GitHubBuildCompleted, StringComparison.InvariantCultureIgnoreCase) &&
                    (action.CheckRun?.Conclusion?.Equals(Constants.GitHubBuildSuccess, StringComparison.InvariantCultureIgnoreCase)?? false) &&
                    buildBranch.Equals(Constants.MasterBranchName, StringComparison.InvariantCultureIgnoreCase) &&
                    (action.CheckRun.CheckSuite?.PullRequests?.Length ?? 0) == 0)
                {
                    await _automationClient.NotifyAsync($"{buildEventNotification} Triggering automation update.");
                    await _automationUpdater.UpdateAsync();
                }
                else
                {
                    await _automationClient.NotifyAsync($"{buildEventNotification} Skipping.");
                }
            }
            finally
            {
                _automationSemaphore.Release();
            }

        }
    }
}
