// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace AspireSeq.AppHost;

public class SeqResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    public string GetConnectionString()
    {
        if (!this.TryGetAnnotationsOfType<AllocatedEndpointAnnotation>(out var seqEndpointAnnotations))
        {
            throw new DistributedApplicationException("Seq resource does not have endpoint annotation.");
        }

        return seqEndpointAnnotations.Single().UriString;
    }
}
