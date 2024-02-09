// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extension methods for connecting a project's OpenTelemetry log events and spans to Seq.
/// </summary>
public static class AspireSeqExtensions
{
    const string ConnectionStringConfigurationKeyPrefix = "ConnectionStrings:";
    const string DefaultConnectionStringConfigurationKey = $"{ConnectionStringConfigurationKeyPrefix}seq";

    public static void AddSeq(this IHostApplicationBuilder builder, string? name = null)
    {
        var connectionString = (builder.Configuration[string.IsNullOrEmpty(name)
            ? DefaultConnectionStringConfigurationKey
            : $"{ConnectionStringConfigurationKeyPrefix}{name}"]) ?? "http://localhost:5341";

        builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri($"{connectionString}/ingest/otlp/v1/logs");
            opt.Protocol = OtlpExportProtocol.HttpProtobuf;
        }));
        builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing
            .AddSource("MyApp.Source")
            .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri($"{connectionString}/ingest/otlp/v1/traces");
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                }
            ));
    }
}
