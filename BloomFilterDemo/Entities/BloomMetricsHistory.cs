namespace BloomFilterDemo.Entities
{
    public class BloomMetricsHistory
    {

        public long Id { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public string InstanceName { get; set; } = string.Empty;

        public long TotalChecks { get; set; }
        public long BloomNegativeCount { get; set; }
        public long BloomPositiveCount { get; set; }
        public long TruePositiveCount { get; set; }
        public long FalsePositiveCount { get; set; }
        public long Inserts { get; set; }

        public double FalsePositiveRateOverPositives { get; set; }
        public double TruePositiveRateOverPositives { get; set; }

    }
}
