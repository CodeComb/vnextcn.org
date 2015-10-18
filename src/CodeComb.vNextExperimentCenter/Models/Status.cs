using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.vNextExperimentCenter.Models
{
    public enum StatusResult
    {
        Queued,
        Building,
        Successful,
        Failed
    }

    public class Status
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public long ProblemId { get; set; }
        public Problem Problem { get; set; }
        public byte[] Archive { get; set; }
        public StatusResult Result { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan TimeUsage { get; set; }
        public long MemoryUsage { get; set; }
        public string Output { get; set; }
    }
}
