using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Supervisor.FilesUpdater
{
    public class GitAutomationRepository : ISourceController
    {
        private ShellCommand _gitShellCommand = new ShellCommand("git");

        public Task<string> GetCurrentBranchNameAsync()
        {
            return _gitShellCommand.RunCommandAsync("rev-parse --abbrev-ref HEAD");
        }

        public async Task<bool> HasUnsynchronizedChangesAsync()
        {
            // Returns true of there are uncommitted changes or
            // if there are unpushed changes.
            return !string.IsNullOrEmpty(await _gitShellCommand.RunCommandAsync("diff HEAD")) ||
                   !string.IsNullOrEmpty(await _gitShellCommand.RunCommandAsync("cherry"));
        }

        public Task UpdateRepositoryAsync()
        {
            return _gitShellCommand.RunCommandAsync("pull");
        }
    }
}
