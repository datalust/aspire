// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Publishing;

namespace Aspire.Hosting;

/// <summary>
///
/// </summary>
public static class SeqBuilderExtensions
{
    const string SeqContainerDataDirectory = "/data";

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="acceptEula"></param>
    /// <param name="name"></param>
    /// <param name="addToManifest"></param>
    /// <param name="port"></param>
    /// <param name="seqDataDirectory"></param>
    /// <returns></returns>
    public static IResourceBuilder<SeqResource> AddSeq(
        this IDistributedApplicationBuilder builder,
        bool acceptEula,
        string name = "seq",
        bool addToManifest = false,
        int port = 5341,
        string? seqDataDirectory = null)
    {
        var seqResource = new SeqResource(name);
        var resourceBuilder = builder.AddResource(seqResource)
            .WithHttpEndpoint(hostPort: port, containerPort: 80)
            .WithAnnotation(new ContainerImageAnnotation {Image = "datalust/seq", Tag = "latest"})
            .WithEnvironment("ACCEPT_EULA", acceptEula ? "Y" : "N");

        if (!string.IsNullOrEmpty(seqDataDirectory))
        {
            resourceBuilder.WithBindMount(seqDataDirectory, SeqContainerDataDirectory);
        }

        if (addToManifest)
        {
            resourceBuilder.WithManifestPublishingCallback(context =>
                WriteSeqResourceToManifest(context, seqResource));
        }
        else
        {
            resourceBuilder.ExcludeFromManifest();
        }

        return resourceBuilder;
    }

    static void WriteSeqResourceToManifest(ManifestPublishingContext context, ContainerResource resource)
    {
        context.WriteContainer(resource);
        context.Writer.WriteString(                     // "connectionString": "...",
            "connectionString",
            $"{{{resource.Name}.bindings.tcp.host}}:{{{resource.Name}.bindings.tcp.port}}");
    }
}
