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

        private readonly HashSet<string> _ongoingBuildExternalIds = new HashSet<string>();

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
                ThrowOnMissingInformation(action);

                if (ShouldSkipAction(action))
                {
                    return;
                }

                if (action.Action!.EqualsIgnoreCase(Constants.GitHubBuildCreated))
                {
                    _ongoingBuildExternalIds.Add(action.CheckRun!.ExternalId!);
                }

                var buildEventStringBuilder = new StringBuilder($"A build was {action.CheckRun!.Status} on branch {action.CheckRun!.CheckSuite!.HeadBranch}.");

                bool shouldUpdate = false;
                if (action.CheckRun!.Status!.EqualsIgnoreCase(Constants.GitHubBuildCompleted))
                {
                    buildEventStringBuilder.Append($" It was a {action.CheckRun!.Conclusion}.");
                    _ongoingBuildExternalIds.Remove(action.CheckRun!.ExternalId!);

                    if (action.CheckRun!.Conclusion!.EqualsIgnoreCase(Constants.GitHubBuildSuccess))
                    {
                        shouldUpdate = action.CheckRun!.CheckSuite!.HeadBranch.EqualsIgnoreCase(Constants.MasterBranchName) &&
                                       !(action.CheckRun.CheckSuite!.PullRequests?.Any() ?? false);
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
                                        string.IsNullOrEmpty(action.CheckRun?.ExternalId) ||
                                        string.IsNullOrEmpty(action.CheckRun!.Status) ||
                                        string.IsNullOrEmpty(action.CheckRun!.CheckSuite?.HeadBranch);

                isMissingInformation |= action.CheckRun!.Status!.EqualsIgnoreCase(Constants.GitHubBuildCompleted) &&
                                        string.IsNullOrEmpty(action.CheckRun!.Conclusion);

            if (isMissingInformation)
            {
                throw new ArgumentException("Received an action that is missing critical information.");
            }
        }

        private bool ShouldSkipAction(GitHubAction action)
        {
            bool shouldSkipAction = action.Action!.EqualsIgnoreCase(Constants.GitHubBuildCreated) &&
                                    _ongoingBuildExternalIds.Contains(action.CheckRun!.ExternalId!);

            return shouldSkipAction;
        }
    }
}
