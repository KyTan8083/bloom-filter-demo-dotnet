namespace BloomFilterDemo.Models
{
    public sealed class BloomMetricsSnapshot
    {
        public long TotalChecks { get; init; }
        public long BloomNegativeCount { get; init; }
        public long BloomPositiveCount { get; init; }
        public long TruePositiveCount { get; init; }
        public long FalsePositiveCount { get; init; }
        public long Inserts { get; init; }

        public double FalsePositiveRateOverPositives =>
            BloomPositiveCount == 0 ? 0 : (double)FalsePositiveCount / BloomPositiveCount;

        public double TruePositiveRateOverPositives =>
            BloomPositiveCount == 0 ? 0 : (double)TruePositiveCount / BloomPositiveCount;
    }
}
