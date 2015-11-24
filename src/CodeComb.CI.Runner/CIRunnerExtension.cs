using CodeComb.CI.Runner;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CIRunnerExtension
    {
        public static IServiceCollection AddCIRunner(this IServiceCollection self,int MaxThreads = 4, int MaxTimeLimit = 1000 * 60 * 20)
        {
            return self.AddSingleton(new CIRunner(MaxThreads, MaxTimeLimit));
        }
    }
}