using System.Collections.Generic;

namespace VideoConverter
{
    public class CodecOption_Double
    {
        public string displayString { get; set; }
        public string cmdString { get; set; }
        public double value { get; set; }
    }

    public class CodecOption_String
    {
        public string displayString { get; set; }
        public string cmdString { get; set; }
        public string Value { get; set; }
    }

    public class CodecOption_Bool
    {
        public string displayString { get; set; }
        public string cmdString { get; set; }
        public bool Value { get; set; }
    }

    public class CodecOption_List
    {
        public string displayString { get; set; }
        public string cmdString { get; set; }
        public List<string> Value { get; set; }
    }
}