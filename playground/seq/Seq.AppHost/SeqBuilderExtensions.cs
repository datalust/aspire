
namespace AspireSeq.AppHost;

public static class SeqBuilderExtensions
{
    public static IResourceBuilder<SeqResource> AddSeq(this IDistributedApplicationBuilder builder, string name, bool acceptEula, int? port = null)
    {
        return builder.AddResource(new SeqResource(name))
            .WithEndpoint(hostPort: 5341, containerPort: 80, scheme: "http")
            .WithAnnotation(new ContainerImageAnnotation { Image = "datalust/seq", Tag = "latest" })
            .WithEnvironment("ACCEPT_EULA", acceptEula ? "Y" : "N");
    }

}

public class SeqResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    public string GetConnectionString()
    {
        if (!this.TryGetAnnotationsOfType<AllocatedEndpointAnnotation>(out var seqEndpointAnnotations))
        {
            throw new DistributedApplicationException("Seq resource does not have endpoint annotation.");
        }

        return seqEndpointAnnotations.Single().EndPointString;
    }
}
