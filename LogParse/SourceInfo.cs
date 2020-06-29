using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParse
{
    public class SourceInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string FullFilename
        {
            get;
            set;
        }

        public string Filename
        {
            get;
            set;
        }

        public SourceInfo(string sFullFilename)
        {
            FileInfo fileInfo = new FileInfo(sFullFilename);
            Name = string.Format("{0}/{1}", fileInfo.Directory.Name, fileInfo.Name);
            FullFilename = sFullFilename;
            Filename = fileInfo.Name;
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
