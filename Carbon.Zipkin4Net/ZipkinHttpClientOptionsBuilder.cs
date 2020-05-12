using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Carbon.Zipkin4Net
{
    public class ZipkinHttpClientOptions
    {
        public string BaseAddress { get; set; }
        public HttpRequestHeaders DefaultRequestHeaders { get; set; }
        public TimeSpan TimeOut { get; set; }
    }

    public class ZipkinHttpClientOptionsBuilder
    {
        private readonly ZipkinHttpClientOptions _options = new ZipkinHttpClientOptions();

        public ZipkinHttpClientOptionsBuilder WithBaseAddress(string baseAddress)
        {
            if (string.IsNullOrEmpty(baseAddress))
            {
                throw new ArgumentNullException("baseAddress");
            }

            _options.BaseAddress = baseAddress;
            return this;
        }
    }
}
