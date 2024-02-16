// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspire.Seq;

public class SeqHealthCheck(string seqUri) : IHealthCheck
{
    readonly HttpClient _client = new() { BaseAddress = new Uri(seqUri) };

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        using var response = await _client.GetAsync("/health", cancellationToken)
            .ConfigureAwait(false);

        return response.IsSuccessStatusCode
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy();
    }
}
