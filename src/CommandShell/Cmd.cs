using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace CommandShell
{
    public class Cmd
    {
        public static Cmd Process(string fileName)
        {
            return new Cmd(fileName, new string[0]);
        }

        public static Cmd Process(string fileName, params string[] arguments)
        {
            return new Cmd(fileName, arguments);
        }

        private ProcessStartInfo info;

        private Cmd(string fileName, string[] arguments)
        {
            info = new ProcessStartInfo(fileName, string.Join(" ", arguments));
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
        }

        public ProcessStartInfo Info { get { return info; } }

        public Cmd UseWorkingDirectory(string path)
        {
            info.WorkingDirectory = path;
            return this;
        }

        public Process Execute(Action<int> callback)
        {
            return Execute((code, output, error) => callback(code));
        }

        public Process Execute(Action<int, string> callback)
        {
            return Execute((code, output, error) => callback(code, output));
        }

        public Process Execute(Action<int, string, string> callback)
        {
            Process process = System.Diagnostics.Process.Start(info);
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) => ExecuteCallback(process, callback);
            return process;
        }

        private void ExecuteCallback(Process process, Action<int, string, string> callback)
        {
            callback(process.ExitCode,
                process.StandardOutput.ReadToEnd(),
                process.StandardError.ReadToEnd());
        }
    }
}
