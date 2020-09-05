using System;
using System.Diagnostics;
using System.Text;
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

            var sbStdErr = new StringBuilder();
            process.ErrorDataReceived += (sender, args) =>
            {
                sbStdErr.Append(args.Data);
            };

            var sbStdOut = new StringBuilder();
            process.OutputDataReceived += (sender, args) =>
            {
                sbStdOut.Append(args.Data);
            };

            process.Start();

            if (await programExitCodeAsync.Task != 0)
            {
                throw new Exception($"error while running {_programName} command with {arguments}. stdout: {sbStdOut}. stderr: {sbStdErr}");
            }

            return Regex.Replace(sbStdOut.ToString(), "[^a-zA-Z]", "");
        }
    }
}
