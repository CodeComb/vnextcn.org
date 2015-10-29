using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CodeComb.Package;

namespace CodeComb.CI.Extractor.Zip
{
    public class UriZipCIExtractor : ICIExtractor
    {
        public UriZipCIExtractor() { }
        public UriZipCIExtractor(string uri, string workingPath)
        {
            Uri = uri;
            WorkingPath = workingPath;
        }

        public string Uri { get; set; }
        public string WorkingPath { get; set; }

        public void Clean()
        {
            Directory.Delete(WorkingPath, true);
        }

        public void Extract()
        {
            Download.DownloadAndExtractAll(Uri, WorkingPath);
        }
    }
}
