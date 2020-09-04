using System.Threading.Tasks;

namespace Supervisor.Automation
{
    public interface IAutomationDeployer
    {
        Task DeployAsync();
    }
}
