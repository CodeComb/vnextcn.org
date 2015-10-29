using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace CodeComb.CI.Runner
{
    public interface ICIRunner
    {
        int MaxTimeLimit { get; set; }
        Task CurrentTask { get; set; }
        Queue<Task> TaskQueue { get; set; }
        int MaxThreads { get; set; }
        IDictionary<string, string> AdditionalEnvironmentVariables { get; set; }
        void PushTask(string Path, dynamic Identifier = null);
        void Polling();
    }
}
