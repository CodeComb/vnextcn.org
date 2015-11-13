using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextChina.Models
{
    public enum ContestStatus
    {
        Pending,
        Live,
        Done
    }

    public class Contest
    {
        [MaxLength(64)]
        public string Id { get; set; }

        [MaxLength(64)]
        public string Title { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public string Description { get; set; }

        [NotMapped]
        public ContestStatus Status
        {
            get
            {
                if (DateTime.Now < Begin)
                    return ContestStatus.Pending;
                else if (DateTime.Now < End)
                    return ContestStatus.Live;
                else
                    return ContestStatus.Done;
            }
        }

        public virtual ICollection<ContestExperiment> Experiments { get; set; } = new List<ContestExperiment>();
    }
}
