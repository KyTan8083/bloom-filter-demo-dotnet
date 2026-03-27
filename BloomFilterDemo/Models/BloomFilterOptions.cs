namespace BloomFilterDemo.Models
{
    public sealed class BloomFilterOptions
    {
        public int BitSize { get; set; } = 1_000_000; // number of bits
        public int HashFunctionCount { get; set; } = 3;
        public string CacheKey { get; set; } = "bloom:primary";
    }
}
