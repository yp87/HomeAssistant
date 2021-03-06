using System;
using System.Threading.Tasks;

namespace Supervisor.Automation
{
    public class DockerComposeHomeAssistantDeployer : IAutomationDeployer
    {
        private readonly ShellCommand _dockerComposeShellCommand = new ShellCommand("docker-compose");

        private readonly IAutomationClient _automationClient;

        public DockerComposeHomeAssistantDeployer(IAutomationClient automationClient)
        {
            _automationClient = automationClient;
        }

        public async Task DeployAsync(bool deployInfrastructure, bool deployAutomation)
        {
            string infrastructuredeploymentOutput = string.Empty;
            if (deployInfrastructure)
            {
                await _automationClient.NotifyAsync("Deploying automation system...");
                infrastructuredeploymentOutput = await _dockerComposeShellCommand.RunCommandAsync("-f docker-compose-homeassistant.yaml --env-file .env up --build -d", true);
            }
            else
            {
                await _automationClient.NotifyAsync("Automation system was not deployed. Nothing changed.");
            }

            if (deployAutomation && (!deployInfrastructure || infrastructuredeploymentOutput.Contains("hass is up-to-date", StringComparison.InvariantCultureIgnoreCase)))
            {
                // Soft restart home assistant to reload configuration.
                await _automationClient.NotifyAsync("Restarting home assistant...");
                await _automationClient.RestartAutomationAsync();
            }
        }
    }
}
