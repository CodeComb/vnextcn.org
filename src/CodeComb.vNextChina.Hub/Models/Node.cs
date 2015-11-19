using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextChina.Hub.Models
{
    [Table("Nodes")]
    public class Node
    {
        public long Id { get; set; }

        [MaxLength(64)]
        public string Alias { get; set; }

        [MaxLength(64)]
        public string Server { get; set; }

        public int Port { get; set; }

        [MaxLength(1024)]
        public string PrivateKey { get; set; }
    }
}
