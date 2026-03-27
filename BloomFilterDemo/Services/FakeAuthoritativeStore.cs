using System.Collections.Concurrent;

namespace BloomFilterDemo.Services
{
    //For demo, we use a thread-safe in-memory set.
    public sealed class FakeAuthoritativeStore : IAuthoritativeStore
    {
        private readonly ConcurrentDictionary<string, byte> _store = new(StringComparer.Ordinal);

        public Task<bool> ExistsAsync(string item, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item);
            return Task.FromResult(_store.ContainsKey(item));
        }

        public Task AddAsync(string item, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item);
            _store[item] = 1;
            return Task.CompletedTask;
        }

        public Task SeedAsync(IEnumerable<string> items, CancellationToken cancellationToken = default)
        {
            foreach (var item in items)
            {
                _store[item] = 1;
            }

            return Task.CompletedTask;
        }
    }
}
