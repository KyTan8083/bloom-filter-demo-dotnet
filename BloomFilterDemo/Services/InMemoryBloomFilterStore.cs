using BloomFilterDemo.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace BloomFilterDemo.Services
{
    //This is the in-memory implementation.
    //Later you can replace it with Redis bitmap operations.

    public sealed class InMemoryBloomFilterStore : IBloomFilterStore
    {
        private readonly IMemoryCache _cache;
        private readonly BloomFilterOptions _options;
        private readonly object _lock = new();

        public InMemoryBloomFilterStore(
            IMemoryCache cache,
            IOptions<BloomFilterOptions> options)
        {
            _cache = cache;
            _options = options.Value;
        }

        public Task<bool> MightContainAsync(string item, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item);

            var bits = GetOrCreateBitArray();

            lock (_lock)
            {
                foreach (var index in GetIndexes(item))
                {
                    if (!bits[index])
                    {
                        return Task.FromResult(false);
                    }
                }
            }

            return Task.FromResult(true);
        }

        public Task AddAsync(string item, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(item);

            var bits = GetOrCreateBitArray();

            lock (_lock)
            {
                foreach (var index in GetIndexes(item))
                {
                    bits[index] = true;
                }
            }

            return Task.CompletedTask;
        }

        private BitArray GetOrCreateBitArray()
        {
            return _cache.GetOrCreate(_options.CacheKey, entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;
                return new BitArray(_options.BitSize);
            })!;
        }

        private IEnumerable<int> GetIndexes(string item)
        {
            // Double hashing approach:
            // index_i = (h1 + i*h2) mod m
            // Good enough for production examples and much better than repeating same hash.
            byte[] bytes = Encoding.UTF8.GetBytes(item);
            byte[] hash = SHA256.HashData(bytes);

            uint h1 = BitConverter.ToUInt32(hash, 0);
            uint h2 = BitConverter.ToUInt32(hash, 4);

            if (h2 == 0)
            {
                h2 = 0x9e3779b9; // avoid zero step
            }

            for (int i = 0; i < _options.HashFunctionCount; i++)
            {
                ulong combined = h1 + ((ulong)i * h2);
                int index = (int)(combined % (ulong)_options.BitSize);
                yield return index;
            }
        }
    }
}
