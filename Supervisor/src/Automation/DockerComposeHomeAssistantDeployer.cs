using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Supervisor.Providers;

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
                infrastructuredeploymentOutput = await _dockerComposeShellCommand.RunCommandAsync("up --build -d homeassistant", true);
            }
            else
            {
                await _automationClient.NotifyAsync("Automation system was not deployed. Nothing changed.");
            }

            if (deployAutomation && (!deployInfrastructure || infrastructuredeploymentOutput.Contains("hass is up-to-date", StringComparison.InvariantCultureIgnoreCase)))
            {
                // Maybe the docker-compose of home assistant did not change, but it is possible
                // that a file in the volume of home assistant changed..
                await _automationClient.NotifyAsync("Restarting home assistant...");
                await _automationClient.RestartAutomationAsync();
            }
        }
    }
}
