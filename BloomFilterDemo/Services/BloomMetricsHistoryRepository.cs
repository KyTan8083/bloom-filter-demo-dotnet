using BloomFilterDemo.Entities;
using Dapper;
using System.Data;

namespace BloomFilterDemo.Services
{
    public sealed class BloomMetricsHistoryRepository : IBloomMetricsHistoryRepository
     {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public BloomMetricsHistoryRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<long> InsertAsync(BloomMetricsHistory entity, CancellationToken cancellationToken = default)
        {
            const string sql = @"
INSERT INTO dbo.BloomMetricsHistory
(
    CreatedAtUtc,
    InstanceName,
    TotalChecks,
    BloomNegativeCount,
    BloomPositiveCount,
    TruePositiveCount,
    FalsePositiveCount,
    Inserts,
    FalsePositiveRateOverPositives,
    TruePositiveRateOverPositives
)
VALUES
(
    @CreatedAtUtc,
    @InstanceName,
    @TotalChecks,
    @BloomNegativeCount,
    @BloomPositiveCount,
    @TruePositiveCount,
    @FalsePositiveCount,
    @Inserts,
    @FalsePositiveRateOverPositives,
    @TruePositiveRateOverPositives
);

SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            using IDbConnection connection = _dbConnectionFactory.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await ((dynamic)connection).OpenAsync(cancellationToken);
            }

            var id = await connection.ExecuteScalarAsync<long>(new CommandDefinition(
                sql,
                entity,
                cancellationToken: cancellationToken));

            return id;
        }

    }
}
