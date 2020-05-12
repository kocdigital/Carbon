using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using zipkin4net;
using zipkin4net.Transport.Http;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Middleware;

namespace Carbon.Zipkin4Net
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseZipkin(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            var zipkin4NetUrl = configuration.GetSection("Zipkin4NetUrl").Value;
            if (!string.IsNullOrEmpty(zipkin4NetUrl))
            {
                var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
                var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
                lifetime.ApplicationStarted.Register(() =>
                {
                    TraceManager.SamplingRate = 1.0f;
                    var logger = new TracingLogger(loggerFactory, "zipkin4net");
                    var httpSender = new HttpZipkinSender(zipkin4NetUrl, "application /json");
                    var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer());
                    TraceManager.RegisterTracer(tracer);
                    TraceManager.Start(logger);
                });
                lifetime.ApplicationStopped.Register(() => TraceManager.Stop());
                app.UseTracing(env.ApplicationName);
            }
            return app;
        }
    }
}
