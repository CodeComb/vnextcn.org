using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.CI.Publisher;
using CodeComb.CI.Publisher.NuGet;

namespace Microsoft.Extensions.DependencyInjection
{ 
    public static class NuGetCIPublisherExtension
    {
        public static IServiceCollection AddNuGetCIPublisher(this IServiceCollection self)
        {
            return self.AddSingleton<ICIPublisher>(x => new NuGetCIPublisher());
        }
    }
}
