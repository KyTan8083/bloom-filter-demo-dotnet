namespace BloomFilterDemo.Models
{
    public sealed class BloomMetricsPersistOptions
    {
        public bool Enabled { get; set; } = true;

        // 每几秒检查一次是否要落库
        public int IntervalSeconds { get; set; } = 60;

        // 节点名称，多实例时有用
        public string InstanceName { get; set; } = Environment.MachineName;

        // 即使数据完全没变化，最多多久强制写一次
        public int ForcePersistIntervalMinutes { get; set; } = 30;
    }
}
