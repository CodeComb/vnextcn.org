using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CodeComb.vNextChina.ViewModels;

namespace CodeComb.vNextChina.Models
{
    public enum ExperimentType
    {
        Develop,
        Test
    }

    
    public enum OSType 
    {
        OSX,
        Windows,
        Linux,
        CrossPlatform,
        Random
    }
    
    public class Experiment
    {
        public long Id { get; set; }

        [MaxLength(128)]
        public string Title { get; set; }

        public string Description { get; set; }

        [MaxLength(128)]
        public string Namespace { get; set; }

        public int Difficulty { get; set; }

        [MaxLength(128)]
        public string Version { get; set; }
        
        public long TimeLimit { get; set; }

        public byte[] AnswerArchive { get; set; }

        public byte[] TestArchive { get; set; }

        public bool CheckPassed { get; set; }

        public string NuGet { get; set; }
        
        public OSType OS { get; set; }

        public int Accepted { get; set; }

        public int Submitted { get; set; }

        [NotMapped]
        public int ACRatio
        {
            get
            {
                if (Submitted == 0)
                    return 0;
                else
                    return Accepted * 100 / Submitted;
            }
        }

        public virtual ICollection<ContestExperiment> Contests { get; set; } = new List<ContestExperiment>();
    }
}
