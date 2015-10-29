using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.CI.Publisher
{
    public interface ICIPublisher
    {
        void Publish(string path, string rules,string apikey, string host);
        IList<string> Discover(string path, string rules);
    }
}
