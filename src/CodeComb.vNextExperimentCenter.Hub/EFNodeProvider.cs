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
        }

        public List<Node> GetNodes()
        {
            var ret = new List<Node>();
            foreach(var x in DB.Nodes.ToList())
            {
                var y = new Node
                {
                    Server = x.Server,
                    Port = x.Port,
                    CurrentThread = 0,
                    MaxThread = 0,
                    PrivateKey = x.PrivateKey,
                    LostConnectionCount = 0
                };
                ret.Add(y);
                y.Init();
            }
            return ret;
        }
    }
}
