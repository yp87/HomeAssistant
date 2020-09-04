using System.Threading.Tasks;

namespace Supervisor.Automation
{
    public interface IAutomationNotifier
    {
        Task SendNotificationAsync(string notificationText);
    }
}
