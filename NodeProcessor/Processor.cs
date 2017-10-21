using Auctus.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Auctus.NodeProcessor
{
    public class Processor
    {
        private int NodeId;

        public Processor(int nodeId)
        {
            NodeId = nodeId;
        }

        internal void Start()
        {
            while (true)
            {
                new ProcessorServices().PostNotSentTransactions(NodeId);
                //TODO:update pendings
                

                //TODO: improve database polling
                Task.Delay(2000);
            }
        }
    }
}
