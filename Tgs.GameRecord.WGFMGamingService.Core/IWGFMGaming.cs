using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameRecord.WGFmGamingService.Core
{
    public interface IWGFMGaming
    {
        ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);

        ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string gameExternalId);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<PlayerModel?> GetPlayerAsync(string playerCode);
    }
}
