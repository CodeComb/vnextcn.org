using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.vNextChina.Models;

namespace CodeComb.vNextChina.ViewModels
{
    public class ExperimentList
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long Accepted { get; set; }
        public long Submitted { get; set; }
        public int ACRatio { get; set; }
        public int Difficulty { get; set; }
        public StatusResult? Flag { get; set; }
    }
}
