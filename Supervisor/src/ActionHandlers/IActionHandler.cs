using Supervisor.Models;
using System.Threading.Tasks;

namespace Supervisor.ActionHandlers
{
    public interface IActionHandler
    {
        bool CanHandleAction(string actionName);

        Task HandleAsync(GitHubAction action);
    }
}
