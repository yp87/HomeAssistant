using System;
using System.Threading.Tasks;

namespace Supervisor.Automation
{
    public class HomeAssistantNotifier : IAutomationNotifier
    {
        public Task SendNotificationAsync(string notificationText)
        {
            // TODO: Send notification to Home Assistant
            Console.WriteLine(notificationText);
            return Task.CompletedTask;
        }
    }
}
