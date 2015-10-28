using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeComb.vNextExperimentCenter.Models
{
    public enum ProblemType
    {
        Develop,
        Test
    }

    
    public enum OSType 
    {
        OSX,
        Windows,
        Linux,
        CrossPlatform
    }
    
    public class Problem
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
    }
}
