using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRecord.WGFmGamingService;
using Tgs.GameRecord.WGFmGamingService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordService(
        this IServiceCollection services,
        Action<RedisOptions, IServiceProvider> redisDBconfigure,
        Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
        Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
        Action<GameInterfaceWGFmGamingClientOptions, IServiceProvider> gameInterfaceWGFmGamingClientOptions)
        => services
            .AddOptions<RedisOptions>()
            .Configure(redisDBconfigure)
            .Services
            .AddSingleton<RedisConnectionManager>()
            .AddMemoryCache()
            .AddGameRecordCore(
                gameRecordInfoOptions,
                gameProvidersQueryServerClientOptions,
                gameInterfaceWGFmGamingClientOptions,
               (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
