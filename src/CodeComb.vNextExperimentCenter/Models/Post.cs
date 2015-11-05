using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextExperimentCenter.Models
{
    public class Post
    {
        public Guid Id { get; set; }

        [ForeignKey("Topic")]
        public long TopicId { get; set; }
        
        public Topic Topic { get; set; }

        public string Content { get; set; }

        public DateTime Time { get; set; }

        [ForeignKey("Parent")]
        public Guid? ParentId { get; set; }

        public Post Parent { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public User User { get; set; }
    }
}
