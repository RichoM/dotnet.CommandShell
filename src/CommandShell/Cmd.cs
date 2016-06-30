using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace RichoM.CommandShell
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

        public Process Execute(Action<int> callback, bool async = true)
        {
            return Execute((code, output, error) => callback(code), async);
        }

        public Process Execute(Action<int, string> callback, bool async = true)
        {
            return Execute((code, output, error) => callback(code, output), async);
        }

        public Process Execute(Action<int, string, string> callback, bool async = true)
        {
            Process process = System.Diagnostics.Process.Start(info);
            if (async)
            {
                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) => ExecuteCallback(process, callback);
            }
            else
            {
                process.WaitForExit();
                ExecuteCallback(process, callback);
            }
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
