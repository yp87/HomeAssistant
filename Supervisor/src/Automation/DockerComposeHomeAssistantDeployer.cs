using System.Threading.Tasks;

namespace Supervisor.Automation
{
    public class DockerComposeHomeAssistantDeployer : IAutomationDeployer
    {
        private ShellCommand _dockerComposeShellCommand = new ShellCommand("docker-compose");

        public async Task DeployAsync()
        {
            await _dockerComposeShellCommand.RunCommandAsync("up --build -d homeassistant");
        }
    }
}
