namespace BloomFilterDemo.Services
{
    //This abstraction lets you use in-memory now and Redis later.
    public interface IBloomFilterStore
    {
        Task<bool> MightContainAsync(string item, CancellationToken cancellationToken = default);
        Task AddAsync(string item, CancellationToken cancellationToken = default);
    }
}
