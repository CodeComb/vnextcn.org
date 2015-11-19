using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.CI.Runner
{
    public class CIList : List<CITask>
    {
        public CITask Dequeue()
        {
            var ret = this.First();
            this.Remove(ret);
            return ret;
        }

        public void Enqueue(CITask task)
        {
            this.Add(task);
        }
    }
}
