using BloomFilterDemo.Entities;

namespace BloomFilterDemo.Services
{
    public interface IBloomMetricsHistoryRepository
    {
        Task<long> InsertAsync(BloomMetricsHistory entity, CancellationToken cancellationToken = default);
    }
}
