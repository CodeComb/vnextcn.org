using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using CodeComb.Package;

namespace CodeComb.vNextChina.Models
{
    public enum TestCaseResult
    {
        Pass,
        Fail,
        Skip
    }

    public class StatusDetail
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Method { get; set; }

        public TestCaseResult Result { get; set; }

        public Package.OSType OS { get; set; }

        [ForeignKey("Status")]
        public long StatusId { get; set; }

        public virtual Status Status { get; set; }

        public float Time { get; set; }
    }
}
