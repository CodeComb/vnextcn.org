using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextChina.Models
{
    public class ContestExperiment
    {
        [ForeignKey("Experiment")]
        public long ExperimentId { get; set; }

        public virtual Experiment Experiment { get; set; }

        [MaxLength(64)]
        [ForeignKey("Contest")]
        public string ContestId { get; set; }

        public virtual Contest Contest { get; set; }

        public int Point { get; set; }
    }
}
