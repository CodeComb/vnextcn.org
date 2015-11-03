using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextExperimentCenter.Models
{
    public class Contest
    {
        [Key]
        [MaxLength(64)]
        public string Id { get; set; }

        [MaxLength(64)]
        public string Title { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public string Description { get; set; }

        public virtual ICollection<ContestExperiment> Experiments { get; set; } = new List<ContestExperiment>();
    }
}
