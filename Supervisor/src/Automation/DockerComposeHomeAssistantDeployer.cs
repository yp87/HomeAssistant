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

        public async Task DeployAsync()
        {
            var output = await _dockerComposeShellCommand.RunCommandAsync("up --build -d homeassistant", true);
            if (output.Contains("hass is up-to-date", StringComparison.InvariantCultureIgnoreCase))
            {
                // Maybe the docker-compose of home assistant did not change, but it is possible
                // that a file in the volume of home assistant changed..
                await _automationClient.NotifyAsync("Restarting home automation...");
                await _automationClient.RestartAutomationAsync();
            }
        }
    }
}
