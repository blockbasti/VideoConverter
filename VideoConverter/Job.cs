using System;
using System.Collections.Generic;

namespace VideoConverter
{
    internal class Job
    {
        public string name { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public string target { get; set; }

        public string codecVideo { get; set; }
        public string codecAudio { get; set; }
        public string resolution { get; set; }
        public string bitrateVideo { get; set; }
        public string bitrateAudio { get; set; }
        public int framerate { get; set; }

        public Dictionary<string, string> information = new Dictionary<string, string>();

        public void fillInformaton()
        {
            //TODO: Implemetierung
            throw new NotImplementedException();
        }
    }
}