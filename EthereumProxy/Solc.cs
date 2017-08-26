using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq;
using Auctus.Util;

namespace Auctus.EthereumProxy
{
    internal class Solc : ConsoleCommand
    {
        private Solc() { }

        private List<SCCompiled> CompiledData { get; set; }
        private string BaseFilePath { get; set; }

        internal static List<SCCompiled> Compile(string name, string smartContractStringified)
        {
            string filePath = string.Format("{0}{1}{2}.sol", Config.GETH_PATH, name, DateTime.UtcNow.Ticks);
            File.WriteAllText(filePath, smartContractStringified);
            try
            {
                Solc solc = new Solc();
                solc.Compile(filePath);
                return solc.CompiledData;
            }
            finally
            {
                File.Delete(filePath);
            }
        }
        
        protected override ConsoleOutput ReadOutput(Process process)
        {
            ConsoleOutput output = base.ReadOutput(process);
            output.Output = output.Output?.Trim('\n', '\r', '\0', ' ');
            return output;
        }
        
        private void Compile(string filePath)
        {
            CompiledData = new List<SCCompiled>();
            BaseFilePath = filePath;

            ConsoleOutput output = Execute(new Command() { Comm = string.Format("solc --optimize --bin {0}", BaseFilePath), ReturnFunction = AfterGenerateBinary });
            if (!output.Ok)
                throw new SolcException(string.Format("Failed to compile ABI.\n\n{0}", output.Output));

            ParseSCAbi(output.Output);
        }

        private Command AfterGenerateBinary(ConsoleOutput output)
        {
            if (!output.Ok)
                throw new SolcException(string.Format("Failed to compile binary.\n\n{0}", output.Output));

            ParseSCBinary(output.Output);
            return new Command() { Comm = string.Format("solc --abi {0}", BaseFilePath) };
        }

        private void ParseSCBinary(string output)
        {
            ParseOutput(output, "Binary:", "Binary");
        }

        private void ParseSCAbi(string output)
        {
            ParseOutput(output, "Contract JSON ABI", "ABI");
        }

        private void ParseOutput(string output, string subtitle, string propertyNameToBeSet)
        {
            string[] initialSplit = output.Split(new string[] { subtitle, "\n", "\r", " ", "=" }, StringSplitOptions.RemoveEmptyEntries);
            SCCompiled scCompiled;
            //Reverse iteration in pairs 
            for (int i = initialSplit.Length - 1; i >= 0; i = i - 2)
            {
                if (initialSplit[i].Contains(BaseFilePath))
                    i = i + 1; //It is a interface, so with empty BIN, go to the next skipping only one
                else
                {
                    string name = initialSplit[i - 1].Split(':').Last();
                    scCompiled = CompiledData.Where(c => c.Name == name).FirstOrDefault();
                    bool newSC = scCompiled == null;
                    if (newSC)
                    {
                        scCompiled = new SCCompiled();
                        scCompiled.Name = name;
                    }
                    scCompiled.GetType().GetProperty(propertyNameToBeSet, BindingFlags.Public | BindingFlags.Instance).SetValue(scCompiled, initialSplit[i]);
                    if (newSC)
                        CompiledData.Add(scCompiled);
                }
            }
        }

        internal class SCCompiled
        {
            public string Name { get; set; }
            public string ABI { get; set; }
            public string Binary { get; set; }
        }
    }
}
