// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Hosting.ApplicationModel;

/// <summary>
///
/// </summary>
/// <param name="name"></param>
public class SeqResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DistributedApplicationException"></exception>
    public string GetConnectionString()
    {
        if (!this.TryGetAnnotationsOfType<AllocatedEndpointAnnotation>(out var seqEndpointAnnotations))
        {
            throw new DistributedApplicationException("Seq resource does not have endpoint annotation.");
        }

        return seqEndpointAnnotations.Single().UriString;
    }
}
