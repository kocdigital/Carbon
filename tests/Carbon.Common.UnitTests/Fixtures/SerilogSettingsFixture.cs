using System.Collections.Generic;
using static Carbon.Common.SerilogSettings;

namespace Carbon.Common.UnitTests.Fixtures
{
    public class SerilogSettingsFixture
    {
        public SerilogSettings SerilogSettings = new SerilogSettings()
        {
            MinimumLevel = "minimum level test",
            Using = new List<string>() { "using test" },
            WriteTo = new List<SerilogSink>()
            {
                new SerilogSink()
                {
                    Args = new Dictionary<string, string>()
                    { {"key","value" } } ,
                    Name ="test serilog"
                }
            }
        };
    }
}
