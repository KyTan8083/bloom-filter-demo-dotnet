namespace BloomFilterDemo.Services
{
    //This represents your real source of truth, such as SQL / MongoDB / service.
    public interface IAuthoritativeStore
    {
        Task<bool> ExistsAsync(string item, CancellationToken cancellationToken = default);
        Task AddAsync(string item, CancellationToken cancellationToken = default);
    }
}
