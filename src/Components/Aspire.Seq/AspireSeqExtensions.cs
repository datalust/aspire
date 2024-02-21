// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire;
using Aspire.Seq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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

    public static void AddSeqEndpoint(this IHostApplicationBuilder builder, string? name = null, Action<SeqSettings>? configureSettings = null)
    {
        var settings = GetSettings(builder, configureSettings);

        var seqUri = (builder.Configuration[string.IsNullOrEmpty(name)
            ? DefaultConnectionStringConfigurationKey
            : $"{ConnectionStringConfigurationKeyPrefix}{name}"]) ?? "http://localhost:5341";

        builder.Services.Configure<OpenTelemetryLoggerOptions>(logging => logging.AddOtlpExporter(opt =>
        {
            opt.Endpoint = new Uri($"{seqUri}/ingest/otlp/v1/logs");
            opt.Protocol = OtlpExportProtocol.HttpProtobuf;
        }));
        builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing
            .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri($"{seqUri}/ingest/otlp/v1/traces");
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                }
            ));

        if (settings.HealthChecks)
        {
            builder.TryAddHealthCheck(new HealthCheckRegistration(
                "Seq.Client",
                _ => new SeqHealthCheck(seqUri),
                failureStatus: default,
                tags: default));
        }
    }

    static SeqSettings GetSettings(this IHostApplicationBuilder builder, Action<SeqSettings>? configureSettings = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var settings = new SeqSettings();
        builder.Configuration.GetSection("Aspire:Seq").Bind(settings);

        configureSettings?.Invoke(settings);
        return settings;
    }
}
