using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.Models
{
    public sealed class BloomMetricsSnapshot
    {
        public long TotalChecks { get; init; }
        public long BloomNegativeCount { get; init; }
        public long BloomPositiveCount { get; init; }
        public long TruePositiveCount { get; init; }
        public long FalsePositiveCount { get; init; }
        public long Inserts { get; init; }
        public double FalsePositiveRateOverPositives { get; init; }
        public double TruePositiveRateOverPositives { get; init; }
    }
}
