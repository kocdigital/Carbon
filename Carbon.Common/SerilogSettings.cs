using System.Collections.Generic;

namespace Carbon.Common
{
    /// <summary>
    /// Contains minimal serilog settings for logging.
    /// </summary>
    /// <remarks>
    /// A Serilog settings provider that reads from <c>Microsoft.Extensions.Configuration</c> sources, including .NET Core's <c>appsettings.json</c> file.
    /// For more information <seealso href="https://github.com/serilog/serilog-settings-configuration"/>
    /// </remarks>
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
