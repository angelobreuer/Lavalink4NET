namespace Lavalink4NET.Logging.Microsoft;

using global::Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicrosoftExtensionsLavalinkLogging(this IServiceCollection services)
    {
        return services.AddSingleton<ILogger, MicrosoftExtensionsLogger>();
    }
}
