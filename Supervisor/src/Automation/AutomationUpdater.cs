using System;
using System.Threading.Tasks;
using Supervisor.FilesUpdater;

namespace Supervisor.Automation
{
    public class AutomationUpdater : IAutomationUpdater
    {
        private readonly IFilesUpdater _filesUpdater;

        private readonly IAutomationNotifier _automationNotifier;

        private readonly IAutomationDeployer _automationDeployer;

        public AutomationUpdater(IFilesUpdater filesUpdater, IAutomationNotifier automationNotifier, IAutomationDeployer automationDeployer)
        {
            _filesUpdater = filesUpdater;
            _automationNotifier = automationNotifier;
            _automationDeployer = automationDeployer;
        }

        public async Task UpdateAsync()
        {
            try
            {
                await _filesUpdater.UpdateFilesAsync();
                await _automationDeployer.DeployAsync();
            }
            catch (Exception e)
            {
                await _automationNotifier.SendNotificationAsync(e.Message);
            }
        }
    }
}
