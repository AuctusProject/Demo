using Auctus.Service;
using Auctus.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.NodeProcessor
{
    public class Processor
    {
        private readonly Cache cache;
        private readonly ILoggerFactory loggerFactory;
        private readonly IConfigurationRoot configuration;
        protected readonly ILogger logger;
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

        internal void Start()
        {
            var taskList = new List<Task>
            {
                Task.Run(() => PostNotSentTransactions()),
                Task.Run(() => ReadPendingTransactions()),
                Task.Run(() => ProcessAutoRecoveryTransactions())
            };
            Task.WaitAny(taskList.ToArray());
            //Log: some task ended and should be restarted
        }

        private void Process(Action<Cache, ILoggerFactory, IConfigurationRoot> action)
        {
            int consecutiveExceptions = 0;
            while (true)
            {
                try
                {
                    action(cache, loggerFactory, configuration);
                    Task.Delay(ExecutionMilisecondsInterval);
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
