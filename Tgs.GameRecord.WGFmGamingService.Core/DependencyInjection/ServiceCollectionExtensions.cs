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
       Action<MongoSettingOptions, IServiceProvider> mongoSettingOptions,
       Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
       Action<GameInterfaceWGFmGamingClientOptions, IServiceProvider> gameInterfaceWGFmGamingClientOptions)
        => services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddOptions<GameRecordInfoOptions>()
            .Configure(gameRecordInfoOptions)
            .Services
            .AddOptions<MongoSettingOptions>()
            .Configure(mongoSettingOptions)
            .Services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddMemoryCache()
            .AddGameInterfaceWGFmGamingServiceClient(gameInterfaceWGFmGamingClientOptions)
            .AddGameCommandClient(gameCommandClientOptions)
            .AddSingleton<RedisConnectionManager>()
            .AddTransient<IScheduleDispatch>(sp => ActivatorUtilities.CreateInstance<ScheduleDispatch>(sp, sp.GetRequiredService<RedisConnectionManager>().Database))
            .AddTransient<IUtensil>(sp => ActivatorUtilities.CreateInstance<UtensilRepository>(sp, TimeSpan.FromHours(1)));
}
