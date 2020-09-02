using System;
using Supervisor.Models;

namespace Supervisor.ActionHandlers
{
    public interface IActionHandler
    {
        bool CanHandleAction(string actionName);

        void Handle(GitHubAction action);
    }
}
