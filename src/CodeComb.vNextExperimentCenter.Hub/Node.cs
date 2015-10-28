using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.vNextExperimentCenter.Hub
{
    public enum NodeStatus
    {
        Free,
        Working,
        Busy,
        Lost
    }

    public class Node
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public int MaxThread { get; set; }
        public int CurrentThread { get; set; }
        public int LostConnectionCount { get; set; } = 0;
        public string PrivateKey { get; set; }
        public NodeStatus Status
        {
            get
            {
                if (LostConnectionCount > 0)
                    return NodeStatus.Lost;
                else if (CurrentThread == 0)
                    return NodeStatus.Free;
                else if (MaxThread > CurrentThread)
                    return NodeStatus.Working;
                else
                    return NodeStatus.Busy;
            }
        }

        public void Init()
        {

        }

        public void HeartBeat()
        {

        }
    }
}
