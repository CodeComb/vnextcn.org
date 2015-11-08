using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.Package;

namespace CodeComb.vNextChina.Hub
{
    public interface INodeProvider
    {
        IList<Node> Nodes { get; set; }
        Node GetFreeNode();
        Node GetFreeNode(OSType OS);
        void Abort(string identifier);
    }
}
