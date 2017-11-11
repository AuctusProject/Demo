using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Auctus.Util.NotShared;

namespace Auctus.EthereumProxy
{
    internal abstract class ConsoleCommand 
    {
        protected ConsoleOutput Execute(string command)
        {
            return Execute(new Command() { Comm = command });
        }

        protected ConsoleOutput Execute(Command command)
        {
            ConsoleOutput output = new ConsoleOutput();
            try
            {
                using (Process process = new Process() { StartInfo = GetBaseStartInfo(command) })
                {
                    if (process.Start())
                        output = ExecuteProcess(process, command);
                    else
                        output.Output = "Error to start Process.";

                    InternalDispose(process);
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

        protected virtual string GetWorkingDirectory()
        {
            return Config.GETH_PATH;
        }

        protected virtual string GetFileName()
        {
            return Config.IS_WINDOWS ? "cmd.exe" : "/bin/bash";
        }
        
        protected virtual string GetStartArgument(Command command)
        {
            return GetValidCommand(command);
        }

        protected virtual string GetFormattedComand(string command)
        {
            string baseCommand = Config.IS_WINDOWS ? "/c{0}" : "-c \"{0}\"";
            return string.Format(baseCommand, command);
        }

        protected virtual ConsoleOutput ExecuteProcess(Process process, Command command)
        {
            ConsoleOutput output = ReadOutput(process);
            return ProcessAllCommands(output, command, process);
        }

        protected virtual void WriteCommand(Process process, Command command)
        {
            process.StartInfo.Arguments = GetValidCommand(command);
            process.Start();
        }

        protected virtual ConsoleOutput ReadOutput(Process process)
        {
            string standard = process.StandardOutput.ReadToEnd();
            process.WaitForExit(15000);
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

        protected virtual void InternalDispose(Process process)
        {

        }

        protected ConsoleOutput ProcessCommand(Process process, Command command)
        {
            WriteCommand(process, command);
            ConsoleOutput output = ReadOutput(process);
            return ProcessAllCommands(output, command, process);
        }

        protected string GetValidCommand(Command command)
        {
            if (command == null || string.IsNullOrWhiteSpace(command.Comm))
                throw new ArgumentNullException("command.Comm", "Command instruction must be filled.");

            return GetFormattedComand(command.Comm);
        }

        private ConsoleOutput ProcessAllCommands(ConsoleOutput currentOutput, Command command, Process process)
        {
            if (command.ReturnFunction != null)
            {
                Command nextCommand = command.ReturnFunction(currentOutput);
                if (nextCommand != null)
                    currentOutput = ProcessCommand(process, nextCommand);
            }
            return currentOutput;
        }

        private ProcessStartInfo GetBaseStartInfo(Command command)
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
                FileName = GetFileName(),
                Arguments = GetStartArgument(command),
                WorkingDirectory = GetWorkingDirectory()
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
