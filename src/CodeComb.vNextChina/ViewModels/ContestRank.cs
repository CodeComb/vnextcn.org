using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.vNextChina.Models;

namespace CodeComb.vNextChina.ViewModels
{
    public class ContestRank
    {
        public User User { get; set; }
        public int Point
        {
            get
            {
                return Details.Sum(x => x.Value.Point);
            }
        }
        public long Time
        {
            get
            {
                return Details.Sum(x => x.Value.TimeUsage);
            }
        }
        public Dictionary<char, ContestRankDetail> Details { get; set; } = new Dictionary<char, ContestRankDetail>();
    }

    public class ContestRankDetail
    {
        public long TimeUsage { get; set; }
        public int Point { get; set; }
    }
}
