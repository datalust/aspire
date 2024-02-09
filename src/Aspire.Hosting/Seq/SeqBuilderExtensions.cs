// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Publishing;

namespace AspireSeq.AppHost;

public static class SeqBuilderExtensions
{
    public static IResourceBuilder<SeqResource> AddSeq(
        this IDistributedApplicationBuilder builder,
        bool acceptEula,
        string name = "seq",
        int port = 5341)
    {
        var seqResource = new SeqResource(name);
        return builder.AddResource(seqResource)
            .WithEndpoint(hostPort: port, containerPort: 80, scheme: "http")
            .WithAnnotation(new ContainerImageAnnotation { Image = "datalust/seq", Tag = "latest" })
            .WithEnvironment("ACCEPT_EULA", acceptEula ? "Y" : "N")
            .WithManifestPublishingCallback(context => WriteSeqResourceToManifest(context, seqResource));
    }

    static void WriteSeqResourceToManifest(ManifestPublishingContext context, SeqResource resource)
    {
        context.WriteContainer(resource);
        context.Writer.WriteString(                     // "connectionString": "...",
            "connectionString",
            $"{{{resource.Name}.bindings.tcp.host}}:{{{resource.Name}.bindings.tcp.port}}");
    }
}
