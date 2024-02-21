// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Seq;

public class SeqSettings
{
    public bool HealthChecks { get; set; } = true;

    public string ApiKey { get; set; } = string.Empty;
}
