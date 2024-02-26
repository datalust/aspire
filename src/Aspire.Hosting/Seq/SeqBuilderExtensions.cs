// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Publishing;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding Seq server resources to the application model.
/// </summary>
public static class SeqBuilderExtensions
{
    // The path within the container in which Seq stores its data
    const string SeqContainerDataDirectory = "/data";

    /// <summary>
    /// Adds a Seq server resource to the application model. A container is used for local development.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name to give the resource.</param>
    /// <param name="addToManifest">Should this resource be added to the manifest.</param>
    /// <param name="port">The host port for the Seq server.</param>
    /// <param name="seqDataDirectory">Host directory to bind to Seq's data directory. This must already exist.</param>
    public static IResourceBuilder<SeqResource> AddSeq(
        this IDistributedApplicationBuilder builder,
        string name = "seq",
        bool addToManifest = false,
        int port = 5341,
        string? seqDataDirectory = null)
    {
        var seqResource = new SeqResource(name);
        var resourceBuilder = builder.AddResource(seqResource)
            .WithHttpEndpoint(hostPort: port, containerPort: 80)
            .WithAnnotation(new ContainerImageAnnotation {Image = "datalust/seq", Tag = "latest"})
            .WithEnvironment("ACCEPT_EULA", "Y");

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
