using System.Collections.Generic;

namespace Carbon.WebApplication
{
    public class CorsPolicySettings
    {
        public IList<string> Origins { get; set; }
        public bool AllowAnyHeaders { get; set; }
        public bool AllowAnyMethods { get; set; }
        public bool AllowAnyOrigin { get; set; }
    }
}
