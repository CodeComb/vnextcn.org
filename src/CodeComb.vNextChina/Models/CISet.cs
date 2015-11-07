using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextChina.Models
{
    public class CISet
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime? LastBuildingTime { get; set; }

        public DateTime CreationTime { get; set; }

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
