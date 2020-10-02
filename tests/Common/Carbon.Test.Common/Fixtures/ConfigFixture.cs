
using Microsoft.Extensions.Configuration;
using System.IO;


namespace Carbon.Test.Common.Fixtures
{
    public  class ConfigFixture
    {
        public IConfigurationRoot GetConfiguration(string path)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile(path, optional: false, reloadOnChange: true)
              .AddEnvironmentVariables();
            return builder.Build();
        }
    }
}
