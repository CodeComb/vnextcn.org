using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace CodeComb.CI.Runner
{
    public class DefaultCIRunner : ICIRunner
    {
        public DefaultCIRunner(int MaxThreads = 4, int MaxTimeLimit = 1000 * 60 * 20)
        {
            this.MaxThreads = MaxThreads;
            this.MaxTimeLimit = MaxTimeLimit;
            Polling();
        }

        private bool Lock = false;

        public IDictionary<string, string> AdditionalEnvironmentVariables { get; set; }

        public int MaxThreads { get; set; }

        public int MaxTimeLimit { get; set; }

        public SecureString Password { get; set; }

        public Queue<Task> TaskQueue { get; set; } = new Queue<Task>();

        public string UserName { get; set; }

        public Task CurrentTask { get; set; }

        private Timer Timer { get; set; }

        public void Polling()
        {
            Timer = new Timer((obj)=> 
            {
                if (Lock) return;
                if (TaskQueue.Count > 0)
                {
                    Lock = true;
                    CurrentTask = TaskQueue.Dequeue();
                    CurrentTask.Run();
                    Lock = false;
                }
            }, this, 0, 5000);
        }

        public void PushTask(string Path, dynamic Identifier = null)
        {
            var task = new Task(this, Path) { Identifier = Identifier };
            TaskQueue.Enqueue(task);
        }
    }
}
