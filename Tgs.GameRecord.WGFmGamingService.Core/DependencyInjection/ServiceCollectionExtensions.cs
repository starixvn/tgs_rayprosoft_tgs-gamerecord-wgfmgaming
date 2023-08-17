using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameRecord.WGFmGamingService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordCore(
       this IServiceCollection services,
       Action<RedisOptions, IServiceProvider> redisDBconfigure,
       Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
       Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
       Action<GameInterfaceWGFmGamingClientOptions, IServiceProvider> gameInterfaceWGFmGamingClientOptions)
        => services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddOptions<GameRecordInfoOptions>()
            .Configure(gameRecordInfoOptions)
            .Services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddMemoryCache()
            .AddGameRecordHandleClient()
            .AddGameInterfaceWGFmGamingServiceClient(gameInterfaceWGFmGamingClientOptions)
            .AddGameCommandClient(gameCommandClientOptions)
            .AddSingleton<RedisConnectionManager>()
            .AddTransient<IScheduleDispatch>(sp => ActivatorUtilities.CreateInstance<ScheduleDispatch>(sp, sp.GetRequiredService<RedisConnectionManager>().Database));
}
