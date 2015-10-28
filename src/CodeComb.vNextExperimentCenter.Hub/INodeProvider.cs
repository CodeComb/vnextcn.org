﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.vNextExperimentCenter.Hub
{
    public interface INodeProvider
    {
        IList<Node> Nodes { get; set; }
        Node GetFreeNode();
    }
}