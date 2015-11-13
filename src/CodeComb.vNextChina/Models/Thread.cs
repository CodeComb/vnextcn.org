using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextChina.Models
{
    public class Thread
    {
        public long Id { get; set; }

        [MaxLength(64)]
        public string Title { get; set; }

        [MaxLength(64)]
        [ForeignKey("Forum")]
        public string ForumId { get; set; }

        public virtual Forum Forum { get; set; }

        public string Content { get; set; }

        public bool IsLocked { get; set; }

        public long Visit { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastReplyTime { get; set; }

        public bool IsTop { get; set; }

        public bool IsAnnouncement { get; set; }
        
        [ForeignKey("User")]
        public long UserId { get; set; }

        public virtual User User { get; set; }

        [NotMapped]
        public virtual Post LastPost { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
