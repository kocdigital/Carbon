using System;
using System.Collections.Generic;

namespace Carbon.Common.UnitTests.Fixtures
{
    public class ApiResponseTestFixture
    {
        public decimal DecimalSampleValue => 3;
        public string StringSampleValue => "Test";
        public DateTime DateTimeSampleValue => DateTime.Now;
        public TimeSpan TimeSpanSampleValue => DateTime.Now - DateTime.Now.AddDays(-1);
        public int IntSpanSampleValue => 2;
        public object ObjectSpanSampleValue => 99;
        public List<string> ListStringSpanSampleValue => new List<string>() { "Test" };
        public int ErrorCodeSample => 5;

    }
}
