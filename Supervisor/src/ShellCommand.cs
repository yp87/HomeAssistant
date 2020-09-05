using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supervisor
{
    public class ShellCommand
    {
        private readonly string _programName;

        public ShellCommand(string programName)
        {
            _programName = programName;
        }

        public async Task<string> RunCommandAsync(string arguments)
        {
            var processStartInfo = new ProcessStartInfo()
            {
                WorkingDirectory = "/Source",
                Arguments = arguments,
                FileName = _programName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            var process = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,

            };

            var programExitCodeAsync = new TaskCompletionSource<int>();
            process.Exited += (sender, args) =>
            {
                programExitCodeAsync.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();

            // Some program like git may write to stderr even if it is not an error.
            // Sometime, it is used for information only.
            string stderr = await process.StandardError.ReadToEndAsync();
            string stdout = await process.StandardOutput.ReadToEndAsync();

            if (await programExitCodeAsync.Task != 0)
            {
                throw new Exception($"error while running {_programName} command with {arguments}. stdout: {stdout}. stderr: {stderr}");
            }

            return Regex.Replace(stdout, "[^a-zA-Z]", "");
        }
    }
}
