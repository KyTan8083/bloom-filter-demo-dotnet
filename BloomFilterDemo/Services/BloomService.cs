namespace BloomFilterDemo.Services
{
    //This is the main service.
    public sealed class BloomService
    {

        private readonly IBloomFilterStore _bloomFilterStore;
        private readonly IAuthoritativeStore _authoritativeStore;
        private readonly BloomMetrics _metrics;
        private readonly ILogger<BloomService> _logger;

        public BloomService(
            IBloomFilterStore bloomFilterStore,
            IAuthoritativeStore authoritativeStore,
            BloomMetrics metrics,
            ILogger<BloomService> logger)
        {
            _bloomFilterStore = bloomFilterStore;
            _authoritativeStore = authoritativeStore;
            _metrics = metrics;
            _logger = logger;
        }

        public async Task AddAsync(string item, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item);

            // Write to source of truth first
            await _authoritativeStore.AddAsync(item, cancellationToken);

            // Then update bloom filter
            await _bloomFilterStore.AddAsync(item, cancellationToken);

            _metrics.RecordInsert();

            _logger.LogInformation("Inserted item {Item} into authoritative store and Bloom filter.", item);
        }

        public async Task<LookupResult> CheckAsync(string item, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item);

            bool mightContain = await _bloomFilterStore.MightContainAsync(item, cancellationToken);

            if (!mightContain)
            {
                _metrics.RecordBloomNegative();

                return new LookupResult
                {
                    Item = item,
                    BloomSaysMightExist = false,
                    ActualExists = false,
                    IsFalsePositive = false,
                    Source = "BloomNegativeFastPath"
                };
            }

            bool actualExists = await _authoritativeStore.ExistsAsync(item, cancellationToken);
            _metrics.RecordBloomPositive(actualExists);

            return new LookupResult
            {
                Item = item,
                BloomSaysMightExist = true,
                ActualExists = actualExists,
                IsFalsePositive = !actualExists,
                Source = "BloomPositiveCheckedAuthoritativeStore"
            };
        }
    }



    public sealed class LookupResult
    {
        public string Item { get; init; } = string.Empty;
        public bool BloomSaysMightExist { get; init; }
        public bool ActualExists { get; init; }
        public bool IsFalsePositive { get; init; }
        public string Source { get; init; } = string.Empty;
    }

}
