using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextExperimentCenter.Models
{
    public class Project
    {
        public Guid Id { get; set; }

        public string Alias { get; set; }

        public string ZipUrl { get; set; }

        public string VersionRule { get; set; }

        public int CurrentVersion { get; set; }

        public string AdditionalEnvironmentVariables { get; set; }

        public string NuGetPrivateKey { get; set; }

        public string NuGetHost { get; set; }

        public int PRI { get; set; }

        public bool RunWithOsx { get; set; }

        public bool RunWithLinux { get; set; }

        public bool RunWithWindows { get; set; }

        [ForeignKey("CISet")]
        public Guid CISetId { get; set; }

        public virtual CISet CISet { get; set; }
    }
}
