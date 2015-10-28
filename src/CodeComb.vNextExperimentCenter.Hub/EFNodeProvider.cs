using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.vNextExperimentCenter.Hub.Models;

namespace CodeComb.vNextExperimentCenter.Hub
{
    public class EFNodeProvider<TContext> : INodeProvider
        where TContext : INodeDbContext
    {
        private TContext DB;

        public EFNodeProvider(TContext db)
        {
            DB = db;
            Nodes = new List<Node>();
            foreach (var x in DB.Nodes.ToList())
            {
                var y = new Node
                {
                    Server = x.Server,
                    Port = x.Port,
                    CurrentThread = 0,
                    MaxThread = 0,
                    PrivateKey = x.PrivateKey,
                    LostConnectionCount = 65536
                };
                Nodes.Add(y);
                y.Init();
            }
        }

        public IList<Node> Nodes { get; set; }

        public Node GetFreeNode()
        {
            return Nodes.Where(x => x.Status != NodeStatus.Lost).OrderBy(x => x.Status).FirstOrDefault();
        }
    }
}
