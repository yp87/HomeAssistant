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
                string modifiedFiles = await _filesUpdater.UpdateFilesAsync();

                bool shouldDeployInfrastructure = ShouldDeployInfrastructure(modifiedFiles);
                bool shouldDeployAutomation = ShouldDeployAutomation(modifiedFiles);

                if (shouldDeployInfrastructure || shouldDeployAutomation)
                {
                    await _automationDeployer.DeployAsync(shouldDeployInfrastructure, shouldDeployAutomation);
                }
                else
                {
                    await _automationClient.NotifyAsync("No critical file changed. Nothing to deploy.");
                }
            }
            catch (Exception e)
            {
                await _automationClient.NotifyAsync(e.Message);
            }
        }

        private bool ShouldDeployAutomation(string modifiedFiles)
        {
            return modifiedFiles.Contains("hass/", StringComparison.InvariantCultureIgnoreCase);
        }

        private bool ShouldDeployInfrastructure(string modifiedFiles)
        {
            return modifiedFiles.Contains("alarm/", StringComparison.InvariantCultureIgnoreCase) ||
                   modifiedFiles.Contains("WebhookProxy/", StringComparison.InvariantCultureIgnoreCase) ||
                   modifiedFiles.Contains("docker-compose.yaml", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
