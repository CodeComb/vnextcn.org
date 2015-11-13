using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextChina.Models
{
    public class Forum
    {
        [MaxLength(64)]
        public string Id { get; set; }

        [MaxLength(128)]
        public string Title { get; set; }

        public bool IsReadOnly { get; set; }

        [MaxLength(64)]
        [ForeignKey("Parent")]
        public string ParentId { get; set; }

        public virtual Forum Parent { get; set; }

        public virtual ICollection<Forum> SubForums { get; set; }

        public int PRI { get; set; }

        public string Description { get; set; }

        public long ThreadCount { get; set; }

        public long PostCount { get; set; }

        [NotMapped]
        public long TodayCount { get; set; }

        [NotMapped]
        public virtual Post LastPost { get; set; }
    }
}
