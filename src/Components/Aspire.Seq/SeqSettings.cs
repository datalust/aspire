// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Seq;

public class SeqSettings
{
    /// <summary>
    /// Should the Seq server participate in health checks.
    /// </summary>
    public bool HealthChecks { get; set; } = true;

    /// <summary>
    /// A Seq <i>API key</i> that authenticates the client to the Seq server.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The base URL of the Seq server (including protocol and port). E.g. "https://example.seq.com:6789"
    /// </summary>
    public string ServerUrl { get; set; } = string.Empty;
}
