using Auctus.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.NodeProcessor
{
    public class Processor
    {
        private readonly int NodeId;

        public Processor(int nodeId)
        {
            NodeId = nodeId;
        }

        internal void Start()
        {
            var taskList = new List<Task>();
            taskList.Add(Task.Run(() => PostNotSentTransactions()));
            taskList.Add(Task.Run(() => ReadPendingTransactions()));
            taskList.Add(Task.Run(() => ProcessAutoRecoveryTransactions()));
            Task.WaitAny(taskList.ToArray());
            //Log: some task ended and should be restarted
        }

        private void Process(Action<int> action)
        {
            while (true)
            {
                try
                {
                    action(NodeId);
                    Task.Delay(2000);
                }
                catch
                {
                    //Ignore exceptions
                }
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
