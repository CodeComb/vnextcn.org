using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextExperimentCenter.Models
{
    public class Forum
    {
        [MaxLength(64)]
        public string Id { get; set; }

        [MaxLength(128)]
        public string Title { get; set; }

        public bool IsReadOnly { get; set; }

        [ForeignKey("Parent")]
        public string ParentId { get; set; }

        public Forum Parent { get; set; }

        public virtual ICollection<Forum> SubForums { get; set; }

        public int PRI { get; set; }
    }
}
