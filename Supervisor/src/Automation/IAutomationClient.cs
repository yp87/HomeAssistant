using System.Threading.Tasks;

namespace Supervisor.Automation
{
    public interface IAutomationClient
    {
        Task NotifyAsync(string notification);

        Task RestartAutomationAsync();
    }
}
