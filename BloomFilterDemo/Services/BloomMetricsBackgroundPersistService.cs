using BloomFilterDemo.Entities;
using BloomFilterDemo.Models;
using Microsoft.Extensions.Options;

namespace BloomFilterDemo.Services
{
    public sealed class BloomMetricsBackgroundPersistService : BackgroundService
    {
        //为什么这里要用 IServiceScopeFactory
        //因为 background service 自己通常是 singleton。
        //但 DbContext 通常是 scoped。
        //到 hosted service 里长期拿着用，这样生命周期不对。

        //所以正确做法是
        //注入 IServiceScopeFactory
        //每次要写 DB 时 CreateScope()
        //从 scope 里拿 repository / DbContext
        private readonly IServiceScopeFactory _serviceScopeFactory;


        private readonly BloomMetrics _metrics;
        private readonly BloomMetricsPersistOptions _options;
        private readonly ILogger<BloomMetricsBackgroundPersistService> _logger;

        private BloomMetricsSnapshot? _lastPersistedSnapshot;
        private DateTime _lastPersistedAtUtc = DateTime.MinValue;

        public BloomMetricsBackgroundPersistService(
            IServiceScopeFactory serviceScopeFactory,
            BloomMetrics metrics,
            IOptions<BloomMetricsPersistOptions> options,
            ILogger<BloomMetricsBackgroundPersistService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _metrics = metrics;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("Bloom metrics persistence is disabled.");
                return;
            }

            _logger.LogInformation(
                "Bloom metrics persistence service started. IntervalSeconds={IntervalSeconds}, InstanceName={InstanceName}, ForcePersistIntervalMinutes={ForcePersistIntervalMinutes}",
                _options.IntervalSeconds,
                _options.InstanceName,
                _options.ForcePersistIntervalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var snapshot = _metrics.Snapshot();

                    bool hasChanges = HasChanges(snapshot, _lastPersistedSnapshot);
                    bool forcePersist = ShouldForcePersist();

                    if (!hasChanges && !forcePersist)
                    {
                        _logger.LogDebug(
                            "Bloom metrics unchanged. Skipping persistence. TotalChecks={TotalChecks}, Inserts={Inserts}, FalsePositiveCount={FalsePositiveCount}",
                            snapshot.TotalChecks,
                            snapshot.Inserts,
                            snapshot.FalsePositiveCount);
                    }
                    else
                    {
                        var entity = new BloomMetricsHistory
                        {
                            CreatedAtUtc = DateTime.UtcNow,
                            InstanceName = _options.InstanceName,
                            TotalChecks = snapshot.TotalChecks,
                            BloomNegativeCount = snapshot.BloomNegativeCount,
                            BloomPositiveCount = snapshot.BloomPositiveCount,
                            TruePositiveCount = snapshot.TruePositiveCount,
                            FalsePositiveCount = snapshot.FalsePositiveCount,
                            Inserts = snapshot.Inserts,
                            FalsePositiveRateOverPositives = snapshot.FalsePositiveRateOverPositives,
                            TruePositiveRateOverPositives = snapshot.TruePositiveRateOverPositives
                        };

                        using var scope = _serviceScopeFactory.CreateScope();
                        var repository = scope.ServiceProvider.GetRequiredService<IBloomMetricsHistoryRepository>();

                        long id = await repository.InsertAsync(entity, stoppingToken);

                        _lastPersistedSnapshot = snapshot;
                        _lastPersistedAtUtc = entity.CreatedAtUtc;

                        _logger.LogInformation(
                            "Bloom metrics snapshot persisted. Id={Id}, CreatedAtUtc={CreatedAtUtc}, TotalChecks={TotalChecks}, FalsePositiveCount={FalsePositiveCount}, FalsePositiveRate={FalsePositiveRate:P2}, Reason={Reason}",
                            id,
                            entity.CreatedAtUtc,
                            entity.TotalChecks,
                            entity.FalsePositiveCount,
                            entity.FalsePositiveRateOverPositives,
                            hasChanges ? "Changed" : "Forced");
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to persist bloom metrics snapshot.");
                }

                await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
            }

            _logger.LogInformation("Bloom metrics persistence service stopped.");
        }




        private bool HasChanges(BloomMetricsSnapshot current, BloomMetricsSnapshot? previous)
        {
            if (previous is null)
            {
                return true;
            }

            return current.TotalChecks != previous.TotalChecks
                || current.BloomNegativeCount != previous.BloomNegativeCount
                || current.BloomPositiveCount != previous.BloomPositiveCount
                || current.TruePositiveCount != previous.TruePositiveCount
                || current.FalsePositiveCount != previous.FalsePositiveCount
                || current.Inserts != previous.Inserts;
        }

        private bool ShouldForcePersist()
        {
            if (_lastPersistedAtUtc == DateTime.MinValue)
            {
                return true;
            }

            var elapsed = DateTime.UtcNow - _lastPersistedAtUtc;
            return elapsed >= TimeSpan.FromMinutes(_options.ForcePersistIntervalMinutes);
        }

    }
}
