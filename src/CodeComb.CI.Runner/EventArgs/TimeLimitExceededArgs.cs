using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.CI.Runner.EventArgs
{
    public class TimeLimitExceededArgs
    {
        public TimeSpan TimeUsage { get; set; }
        public string Output { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ExitTime { get; set; }
        public long PeakMemoryUsage { get; set; }
        public int ExitCode { get; set; }
    }
}
