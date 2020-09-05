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

        public async Task<string> RunCommandAsync(string arguments, bool appendError = false)
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
            };

            process.Start();

            // Some program like git may write to stderr even if it is not an error.
            // Sometime, it is used for information only.
            string stderr = await process.StandardError.ReadToEndAsync();
            string stdout = await process.StandardOutput.ReadToEndAsync();

            await Task.Run(() => process.WaitForExit());

            if (process.ExitCode != 0)
            {
                throw new Exception($"error while running {_programName} command with {arguments}. stdout: {stdout}. stderr: {stderr}");
            }

            return appendError ? $"{stdout}{stderr}" : stdout;
        }
    }
}
