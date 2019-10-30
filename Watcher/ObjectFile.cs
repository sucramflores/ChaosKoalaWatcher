using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Watcher
{
    public class ObjectFile
    {
        public string NameFile { get; set; }
        public string NameFileFull { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime Created { get; set; }
        public int NumberOfLines { get; set; }



    }
}
