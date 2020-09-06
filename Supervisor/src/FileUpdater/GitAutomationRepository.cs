using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supervisor.FilesUpdater
{
    public class GitAutomationRepository : ISourceController
    {
        private ShellCommand _gitShellCommand = new ShellCommand("git");

        public async Task<string> GetCurrentBranchNameAsync()
        {
            string result = await _gitShellCommand.RunCommandAsync("rev-parse --abbrev-ref HEAD");
            return Regex.Replace(result, "[^a-zA-Z]", "");
        }

        public async Task<bool> HasUnsynchronizedChangesAsync()
        {
            // Returns true of there are uncommitted changes or
            // if there are unpushed changes
            return !string.IsNullOrEmpty(await _gitShellCommand.RunCommandAsync("diff HEAD")) ||
                   !string.IsNullOrEmpty(await _gitShellCommand.RunCommandAsync("cherry"));
        }

        public async Task<string> UpdateRepositoryAsync()
        {
            string commitIdBeforeUpdate = await _gitShellCommand.RunCommandAsync("log --format=\"%H\" -n 1");
            await _gitShellCommand.RunCommandAsync("pull");
            return await _gitShellCommand.RunCommandAsync($"diff --name-only {commitIdBeforeUpdate}");
        }
    }
}
