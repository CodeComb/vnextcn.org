using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CodeComb.Package;

namespace CodeComb.CI.Extractor.Zip
{
    public class ZipCIExtractor : ICIExtractor
    {
        public ZipCIExtractor(string zipFile, string workingPath)
        {
            ZipFile = zipFile;
            WorkingPath = workingPath;
        }

        public string ZipFile { get; set; }
        public string WorkingPath { get; set; }

        public void Clean()
        {
            Directory.Delete(WorkingPath, true);
        }

        public void Extract()
        {
            Unzip.ExtractAll(ZipFile, WorkingPath);
        }
    }
}
