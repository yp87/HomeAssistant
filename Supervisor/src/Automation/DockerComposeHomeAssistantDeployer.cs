using System;
using System.Threading.Tasks;

namespace Supervisor.Automation
{
    public class DockerComposeHomeAssistantDeployer : IAutomationDeployer
    {
        private ShellCommand _dockerComposeShellCommand = new ShellCommand("docker-compose");

        public async Task DeployAsync()
        {
            var output = await _dockerComposeShellCommand.RunCommandAsync("up --build -d homeassistant", true);
            if (output.Contains("hass is up-to-date", StringComparison.InvariantCultureIgnoreCase))
            {
                // Maybe the docker-compose of home assistant did not change, but it is possible
                // that a file in the volume of home assistant changed..
                // TODO: Send restart request to Home assistant directly instead of restarting the container..
                await _dockerComposeShellCommand.RunCommandAsync("restart homeassistant");
            }
        }
    }
}
