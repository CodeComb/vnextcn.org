using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.Package;
using CodeComb.vNextChina.Hub.Models;

namespace CodeComb.vNextChina.Hub
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
                    PrivateKey = x.PrivateKey,
                    LostConnectionCount = 1,
                    Alias = x.Alias
                };
                y.Init();
                Nodes.Add(y);
            }
        }

        public IList<Node> Nodes { get; set; }

        public Node GetFreeNode()
        {
            return Nodes.Where(x => x.Status != NodeStatus.Lost).OrderBy(x => x.Status).FirstOrDefault();
        }

        public Node GetFreeNode(OSType OS)
        {
            return Nodes.Where(x => x.Status != NodeStatus.Lost && x.OS == OS).OrderBy(x => x.Status).FirstOrDefault();
        }

        public void Abort(string Identifier)
        {
            foreach (var x in Nodes)
                x.Abort(Identifier);
        }
    }
}
