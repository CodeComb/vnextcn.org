using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.CI.Runner.EventArgs
{
    public class TestCaseArgs
    {
        public string Title { get; set; }
        public float Time { get; set; }
        public string Method { get; set; }
        public string Result { get; set; }
    }
}
