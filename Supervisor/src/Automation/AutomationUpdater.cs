using System;
using System.Threading.Tasks;
using Supervisor.FilesUpdater;

namespace Supervisor.Automation
{
    public class AutomationUpdater : IAutomationUpdater
    {
        private readonly IFilesUpdater _filesUpdater;

        private readonly IAutomationClient _automationClient;

        private readonly IAutomationDeployer _automationDeployer;

        public AutomationUpdater(IFilesUpdater filesUpdater, IAutomationClient automationClient, IAutomationDeployer automationDeployer)
        {
            _filesUpdater = filesUpdater;
            _automationClient = automationClient;
            _automationDeployer = automationDeployer;
        }

        public async Task UpdateAsync()
        {
            try
            {
                await _automationClient.NotifyAsync("Updating files...");
                await _filesUpdater.UpdateFilesAsync();
                await _automationClient.NotifyAsync("Deploying automation server...");
                await _automationDeployer.DeployAsync();
            }
            catch (Exception e)
            {
                await _automationClient.NotifyAsync(e.Message);
            }
        }
    }
}
