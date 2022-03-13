namespace Lavalink4NET.Logging.NLog;

using global::Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNLogLavalinkLogging(this IServiceCollection services)
    {
        return services.AddSingleton<ILogger, NLogExtensionsLogger>();
    }
}
