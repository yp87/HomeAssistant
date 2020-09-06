using System.Threading.Tasks;

namespace Supervisor.Automation
{
    public interface IAutomationDeployer
    {
        Task DeployAsync(bool deployInfrastructure, bool deployAutomation);
    }
}
