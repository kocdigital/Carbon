using Carbon.Common.UnitTests.Fixtures;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Carbon.Common.UnitTests
{
    public class SerilogSettingsTest : IClassFixture<SerilogSettingsFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly SerilogSettingsFixture _serilogSettingFixture;
        public SerilogSettingsTest(ITestOutputHelper testOutputHelper, SerilogSettingsFixture serilogSettingFixture)
        {
            _testOutputHelper = testOutputHelper;
            _serilogSettingFixture = serilogSettingFixture;
        }
        [Fact]
        public void SetValuesToOrdarableDto_Successfully_OrdarableDto()
        {
            // Arrange
            // Act
            var setting = _serilogSettingFixture.SerilogSettings;
            // Assert      
            Assert.NotNull(setting.MinimumLevel);
            Assert.NotNull(setting.Using);
            Assert.NotNull(setting.WriteTo);
            Assert.NotNull(setting.WriteTo.First().Name);
            Assert.NotNull(setting.WriteTo.First().Args);

            _testOutputHelper.WriteLine("Test passed!");
        }
    }
}
