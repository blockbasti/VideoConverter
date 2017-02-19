using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoConverter
{
    internal class Job
    {
        public string name { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public string target { get; set; }
    }
}