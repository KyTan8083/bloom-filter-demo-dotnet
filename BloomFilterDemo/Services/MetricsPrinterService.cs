namespace BloomFilterDemo.Services
{
    //This prints real-time metrics.


    public sealed class MetricsPrinterService : BackgroundService
    {
        private readonly BloomMetrics _metrics;
        private readonly ILogger<MetricsPrinterService> _logger;

        public MetricsPrinterService(
            BloomMetrics metrics,
            ILogger<MetricsPrinterService> logger)
        {
            _metrics = metrics;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var snapshot = _metrics.Snapshot();

                _logger.LogInformation(
                    "Bloom Metrics => TotalChecks: {TotalChecks}, BloomNegative: {BloomNegative}, BloomPositive: {BloomPositive}, TruePositive: {TruePositive}, FalsePositive: {FalsePositive}, FP Rate (positive path): {FalsePositiveRate:P2}, Inserts: {Inserts}",
                    snapshot.TotalChecks,
                    snapshot.BloomNegativeCount,
                    snapshot.BloomPositiveCount,
                    snapshot.TruePositiveCount,
                    snapshot.FalsePositiveCount,
                    snapshot.FalsePositiveRateOverPositives,
                    snapshot.Inserts);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
