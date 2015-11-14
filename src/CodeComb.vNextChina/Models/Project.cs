using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextChina.Models
{
    public enum ProjectRestoreMethod
    {
        Zip,
        Git,
        Svn
    }

    public enum ProjectPublishMethod
    {
        None,
        NuGet,
        WebDeploy
    }

    public class Project
    {
        public Guid Id { get; set; }

        public string Alias { get; set; }

        public string Url { get; set; }

        [MaxLength(64)]
        public string Branch { get; set; }

        public string VersionRule { get; set; }

        public int CurrentVersion { get; set; }

        public string AdditionalEnvironmentVariables { get; set; }

        public string NuGetPrivateKey { get; set; }

        public string NuGetHost { get; set; }

        public int PRI { get; set; }

        public bool RunWithOsx { get; set; }

        public bool RunWithLinux { get; set; }

        public bool RunWithWindows { get; set; }

        public ProjectRestoreMethod RestoreMethod { get; set; }

        public ProjectPublishMethod PublishMethod { get; set; }

        [ForeignKey("CISet")]
        public Guid CISetId { get; set; }

        public virtual CISet CISet { get; set; }
    }
}
