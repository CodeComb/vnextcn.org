using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.vNextChina.ViewModels
{
    public class ContestStatistics
    {
        public string Title { get; set; }
        public long ExperimentId { get; set; }
        public long Accepted { get; set; }
        public long Submitted { get; set; }
        public int Point { get; set; }
        public double Average { get; set; }
        public string Flag { get; set; }
        public char Number { get; set; }
    }
}
