using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.WGFmGamingService.Client;
using Tgs.GameRecord.WGFmGaming;
using Tgs.GameRecord.WGFmGaming.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecord(
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
        .AddSingleton<RedisConnectionManager>()
        .AddGameRecordCore(
            gameCommandClientOptions: gameCommandClientOptions,
            gameInterfaceWGFmGamingClientOptions: gameInterfaceWGFmGamingClientOptions,
            redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
