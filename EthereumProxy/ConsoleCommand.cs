using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Auctus.EthereumProxy
{
    internal abstract class ConsoleCommand 
    {
        protected static readonly bool IS_WINDOWS = false;

        protected ConsoleOutput Execute(string command)
        {
            return Execute(new Command() { Comm = command });
        }

        protected ConsoleOutput Execute(Command command)
        {
            ConsoleOutput output = new ConsoleOutput();
            try
            {
                using (Process process = new Process() { StartInfo = getBaseStartInfo(command) })
                {
                    if (process.Start())
                        output = executeProcess(process, command);
                    else
                        output.Output = "Error to start Process.";

                    process.StandardOutput.Dispose();
                    process.StandardInput.Dispose();
                    process.StandardError.Dispose();
                }
            }
            catch (Exception e)
            {
                output.Success = false;
                output.Output = e.GetBaseException().Message;
            }
            return output;
        }

        protected virtual string getFileName()
        {
            return IS_WINDOWS ? Util.Config.WindowsCmdExec : Util.Config.LinuxCmdExec;
        }

        protected virtual string getWorkingDirectory()
        {
            return IS_WINDOWS ? Util.Config.WindowsWorkingDir : Util.Config.LinuxWorkingDir;
        }

        protected virtual string getStartArgument(Command command)
        {
            return getValidCommand(command);
        }

        protected virtual string getFormattedComand(string command)
        {
            return string.Format("{1}{0}", IS_WINDOWS ?  "/c" : "-c", command);
        }

        protected virtual ConsoleOutput executeProcess(Process process, Command command)
        {
            ConsoleOutput output = readOutput(process);
            return processAllCommands(output, command, process);
        }

        protected virtual void writeCommand(Process process, Command command)
        {
            process.StartInfo.Arguments = getValidCommand(command);
            process.Start();
        }

        protected virtual ConsoleOutput readOutput(Process process)
        {
            string standard = process.StandardOutput.ReadToEnd();
            ConsoleOutput output = new ConsoleOutput();
            output.Success = process.HasExited && process.ExitCode == 0;
            if (output.Success)
                output.Output = standard;
            else
            {
                string error = process.StandardError.ReadToEnd();
                output.Output = (string.IsNullOrEmpty(standard) ? error : (string.IsNullOrEmpty(error) ? standard : standard + '\r' + '\n' + error));
            }
            return output;
        }

        protected ConsoleOutput processCommand(Process process, Command command)
        {
            writeCommand(process, command);
            ConsoleOutput output = readOutput(process);
            return processAllCommands(output, command, process);
        }

        protected string getValidCommand(Command command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.Comm))
                throw new ArgumentNullException("command.Comm", "Command instruction must be filled.");

            return getFormattedComand(command.Comm);
        }

        private ConsoleOutput processAllCommands(ConsoleOutput currentOutput, Command command, Process process)
        {
            if (command.ReturnFunction != null)
            {
                Command nextCommand = command.ReturnFunction(currentOutput);
                if (nextCommand != null)
                    currentOutput = processCommand(process, nextCommand);
            }
            return currentOutput;
        }

        private ProcessStartInfo getBaseStartInfo(Command command)
        {
            return new ProcessStartInfo()
            {
                RedirectStandardInput = true,
                RedirectStandardError = true,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = getFileName(),
                Arguments = getStartArgument(command),
                WorkingDirectory = getWorkingDirectory()
            };
        }

        protected class ConsoleOutput
        {
            public bool Success { get; set; }
            public string Output { get; set; }

            public ConsoleOutput() { }

            public ConsoleOutput(bool success, string output)
            {
                Success = success;
                Output = output;
            }

            public bool Ok { get { return Success && !string.IsNullOrEmpty(Output); } }
        }

        protected class Command
        {
            public string Comm { get; set; }
            public Func<ConsoleOutput, Command> ReturnFunction { get; set; }
        }
    }
}
