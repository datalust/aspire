// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

public static class SeqExtensions
{
    public static void MapSeqApi(this WebApplication app)
    {
        app.MapGet("/seq/verify", VerifySeqAsync);
    }

    private static Task<IResult> VerifySeqAsync()
    {
        return Task.FromResult(Results.Ok());
    }
}
