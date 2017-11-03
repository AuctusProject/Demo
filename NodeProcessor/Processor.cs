using Auctus.Service;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auctus.NodeProcessor
{
    public class Processor
    {
        private readonly Cache cache;
        private readonly ILoggerFactory loggerFactory;
        private readonly IConfigurationRoot configuration;
        protected readonly ILogger logger;
        private string processName;
        private int ExecutionMilisecondsInterval {
            get {
                if(configuration!=null && configuration["ExecutionMilisecondsInterval"] !=null)
                    return Convert.ToInt32(configuration["ExecutionMilisecondsInterval"]);
                return 2000;
            }
        }

        public Processor(ILoggerFactory loggerFactory, IConfigurationRoot configuration, Cache cache)
        {
            this.cache = cache;
            this.loggerFactory = loggerFactory;
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger(GetType().Namespace);
        }

        internal void Start(string [] args)
        {
            var process = GetProcessToRun(args);
            process();
        }

        private Action GetProcessToRun(string[] args)
        {
            processName = args[0];
            switch (processName)
            {
                case "ProcessPensionFundsEntries":
                    return ProcessPensionFundsEntries;
                case "ReadContractMined":
                    return ReadContractMined;
                case "PostNotSentTransactions":
                    return PostNotSentTransactions;
                case "ReadPendingTransactions":
                    return ReadPendingTransactions;
                case "ProcessAutoRecoveryTransactions":
                    return ProcessAutoRecoveryTransactions;
                default:
                {
                    logger.LogError($"Invalid process: {processName}");
                        throw new ArgumentException("args");
                }
            }
        }

        private void Process(Action<Cache, ILoggerFactory, IConfigurationRoot> action)
        {
            int consecutiveExceptions = 0;
            while (true)
            {
                try
                {
                    logger.LogInformation($"Method {processName} started");
                    action(cache, loggerFactory, configuration);
                    logger.LogInformation($"Method {processName} ended");
                    Thread.Sleep(ExecutionMilisecondsInterval);
                    consecutiveExceptions = 0;
                }
                catch (Exception e)
                {
                    consecutiveExceptions++;
                    SafeLog(e);
                    if(consecutiveExceptions > 5)
                    {
                        //Force application exception for restart
                        throw;
                    }
                }
            }
        }

        private void SafeLog(Exception e)
        {
            try
            {
                logger.LogError(e.ToString());
            }
            catch
            {
                //ignore
            }
        }

        public void ProcessPensionFundsEntries()
        {
            Process(new ProcessorServices().ProcessPensionFundsEntries);
        }

        public void ReadContractMined()
        {
            Process(new ProcessorServices().ReadContractMined);
        }

        public void PostNotSentTransactions()
        {
            Process(new ProcessorServices().PostNotSentTransactions);
        }

        public void ReadPendingTransactions()
        {
            Process(new ProcessorServices().ReadPendingTransactions);
        }

        public void ProcessAutoRecoveryTransactions()
        {
            Process(new ProcessorServices().ProcessAutoRecoveryTransactions);
        }
    }
}
