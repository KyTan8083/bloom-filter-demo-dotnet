using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.Models
{
    public sealed class LookupResult
    {
        public string Item { get; init; } = string.Empty;
        public bool BloomSaysMightExist { get; init; }
        public bool ActualExists { get; init; }
        public bool IsFalsePositive { get; init; }
        public string Source { get; init; } = string.Empty;
    }
}
