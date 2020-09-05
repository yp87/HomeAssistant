using System;
using System.Diagnostics;
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

            string error = await process.StandardError.ReadToEndAsync();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"error while running {_programName} command with {arguments} : {error}");
            }

            return await process.StandardOutput.ReadToEndAsync();
        }
    }
}
