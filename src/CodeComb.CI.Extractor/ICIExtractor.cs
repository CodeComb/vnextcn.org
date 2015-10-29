using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeComb.CI.Extractor
{
    public interface ICIExtractor
    {
        void Extract();
        void Clean();
    }
}
