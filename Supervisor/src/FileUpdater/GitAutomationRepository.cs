using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Supervisor.FilesUpdater
{
    public class GitAutomationRepository : ISourceController
    {
        public Task<string> GetCurrentBranchNameAsync()
        {
            return RunGitCommandAsync("rev-parse --abbrev-ref HEAD");
        }

        public async Task<bool> HasUnsynchronizedChangesAsync()
        {
            // Returns true of there are uncommitted changes or
            // if there are unpushed changes
            return !string.IsNullOrEmpty(await RunGitCommandAsync("diff HEAD")) ||
                   !string.IsNullOrEmpty(await RunGitCommandAsync("cherry"));
        }

        public Task UpdateRepositoryAsync()
        {
            return RunGitCommandAsync("pull");
        }

        private async Task<string> RunGitCommandAsync(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                WorkingDirectory = "/Source",
                FileName = "git",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            Process proc = new Process()
            {
                StartInfo = startInfo,
            };

            proc.Start();

            string error = await proc.StandardError.ReadToEndAsync();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"error while running git command with {arguments} : {error}");
            }

            return await proc.StandardOutput.ReadToEndAsync();
        }
    }
}
