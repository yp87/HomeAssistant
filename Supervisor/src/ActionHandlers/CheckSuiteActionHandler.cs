using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Supervisor.Automation;
using Supervisor.Models;

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
            actionName.EqualsIgnoreCase("check_suite");

        public async Task HandleAsync(GitHubAction action)
        {
            await _automationSemaphore.WaitAsync();
            try
            {
                ThrowOnMissingInformation(action);

                var buildEventStringBuilder = new StringBuilder($"A build was {action.CheckSuite!.Status} on branch {action.CheckSuite!.HeadBranch}.");

                bool shouldUpdate = false;
                if (action.CheckSuite!.Status!.EqualsIgnoreCase(Constants.GitHubBuildCompleted))
                {
                    buildEventStringBuilder.Append($" It was a {action.CheckSuite!.Conclusion}.");

                    if (action.CheckSuite!.Conclusion!.EqualsIgnoreCase(Constants.GitHubBuildSuccess))
                    {
                        shouldUpdate = action.CheckSuite!.HeadBranch.EqualsIgnoreCase(Constants.MasterBranchName) &&
                                       !(action.CheckSuite!.PullRequests?.Any() ?? false);
                        buildEventStringBuilder.Append(shouldUpdate ? " Triggering update." : " Skipping.");
                    }
                }

                await _automationClient.NotifyAsync(buildEventStringBuilder.ToString());

                if (shouldUpdate)
                {
                    await _automationUpdater.UpdateAsync();
                }
            }
            catch (Exception e)
            {
                await _automationClient.NotifyAsync($"An {e.GetType().Name} occured while handling github action: {e.Message}");
            }
            finally
            {
                _automationSemaphore.Release();
            }
        }

        private void ThrowOnMissingInformation(GitHubAction action)
        {
            bool isMissingInformation = string.IsNullOrEmpty(action.Action) ||
                                        string.IsNullOrEmpty(action.CheckSuite?.Status) ||
                                        string.IsNullOrEmpty(action.CheckSuite!.HeadBranch);

                isMissingInformation |= action.CheckSuite!.Status!.EqualsIgnoreCase(Constants.GitHubBuildCompleted) &&
                                        string.IsNullOrEmpty(action.CheckSuite!.Conclusion);

            if (isMissingInformation)
            {
                throw new ArgumentException("Received an action that is missing critical information.");
            }
        }
    }
}
