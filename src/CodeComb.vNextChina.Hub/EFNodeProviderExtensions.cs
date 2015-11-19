using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeComb.vNextChina.Hub;
using CodeComb.vNextChina.Hub.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EFProviderExtensions
    {
        public static IServiceCollection AddEFNodeProvider<TContext>(this IServiceCollection self)
            where TContext: INodeDbContext
        {
            return self.AddSingleton<INodeProvider, EFNodeProvider<TContext>>();
        }
    }
}
