using System.Collections.Generic;

namespace Carbon.Common
{
    public class SerilogSettings
    {
        public IList<string> Using { get; set; }
        public string MinimumLevel { get; set; }
        public IList<SerilogSink> WriteTo { get; set; }

        public class SerilogSink
        {
            public string Name { get; set; }
            public IDictionary<string, string> Args{ get; set; }
        }
    }


}
