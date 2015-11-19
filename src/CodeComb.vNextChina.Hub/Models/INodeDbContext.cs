using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace CodeComb.vNextChina.Hub.Models
{
    public interface INodeDbContext
    {
        DbSet<Node> Nodes { get; set; }
        int SaveChanges();
    }
}
