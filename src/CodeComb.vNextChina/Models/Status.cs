using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextChina.Models
{
    public enum StatusType
    {
        Experiment,
        Project
    }

    public enum StatusResult
    {
        Ignored,
        Queued,
        Building,
        Successful,
        Failed
    }

    public class Status
    {
        public long Id { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey("Experiment")]
        public long? ExperimentId { get; set; }

        public virtual Experiment Experiment { get; set; }

        [ForeignKey("Project")]
        public Guid? ProjectId { get; set; }
        
        public virtual Project Project { get; set; }

        public byte[] Archive { get; set; }

        public StatusResult Result { get; set; }

        public bool RunWithWindows { get; set; }

        public bool RunWithOsx { get; set; }

        public bool RunWithLinux { get; set; }

        public StatusResult WindowsResult { get; set; }

        public StatusResult OsxResult { get; set; }

        public StatusResult LinuxResult { get; set; }

        public DateTime Time { get; set; }

        public long TimeUsage { get; set; }

        public long MemoryUsage { get; set; }

        public string LinuxOutput { get; set; }

        public string OsxOutput { get; set; }

        public string WindowsOutput { get; set; }

        public string NuGet { get; set; }

        public int Accepted { get; set; }

        public int Total { get; set; }

        public StatusType Type { get; set; }

        public virtual ICollection<StatusDetail> Details { get; set; } = new List<StatusDetail>();

        public StatusResult GenerateResult()
        {
            if ((this.WindowsResult != Models.StatusResult.Building && this.WindowsResult != Models.StatusResult.Queued)
                && (this.LinuxResult != Models.StatusResult.Building && this.LinuxResult != Models.StatusResult.Queued)
                && (this.OsxResult != Models.StatusResult.Building && this.OsxResult != Models.StatusResult.Queued))
            {
                Console.WriteLine((StatusResult)Math.Max(Math.Max((int)this.WindowsResult, (int)this.LinuxResult), (int)this.OsxResult));
                return (StatusResult)Math.Max(Math.Max((int)this.WindowsResult, (int)this.LinuxResult), (int)this.OsxResult);
            }
            else if (this.WindowsResult == StatusResult.Building || this.LinuxResult == StatusResult.Building || this.OsxResult == StatusResult.Building)
            {
                return StatusResult.Building;
            }
            else if (this.WindowsResult == StatusResult.Queued || this.LinuxResult == StatusResult.Queued || this.OsxResult == StatusResult.Queued)
            {
                return StatusResult.Queued;
            }
            else
            {
                return StatusResult.Ignored;
            }
        }
    }
}
