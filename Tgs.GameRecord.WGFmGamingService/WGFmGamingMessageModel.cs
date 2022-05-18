namespace Tgs.GameRecord.WGFmGamingService
{
    public record WGFmGamingMessageModel
    {
        public string GameProviderCode { get; init; } = default!;
        public DateTimeOffset SendDateTime { get; init; }
    }
}
