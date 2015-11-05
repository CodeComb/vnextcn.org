using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.vNextExperimentCenter.Models;

namespace CodeComb.vNextExperimentCenter.ViewModels
{
    public class CIProject
    {
        public CIProject() { }

        public CIProject(Project project, Status status)
        {
            Id = project.Id;
            Alias = project.Alias;
            Version = "-";
            if (status != null)
            {
                Time = status.Time;
                StatusId = status.Id;
                Version = string.Format(project.VersionRule, project.CurrentVersion);
            }
        }

        public Guid Id { get; set; }

        public string Alias { get; set; }

        public long? StatusId { get; set; }

        public DateTime Time { get; set; }

        public string Version { get; set; }
    }
}
