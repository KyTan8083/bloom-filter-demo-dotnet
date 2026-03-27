
    CREATE TABLE dbo.BloomMetricsHistory
    (
        Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CreatedAtUtc DATETIME2(3) NOT NULL,
        InstanceName NVARCHAR(200) NOT NULL,

        TotalChecks BIGINT NOT NULL,
        BloomNegativeCount BIGINT NOT NULL,
        BloomPositiveCount BIGINT NOT NULL,
        TruePositiveCount BIGINT NOT NULL,
        FalsePositiveCount BIGINT NOT NULL,
        Inserts BIGINT NOT NULL,

        FalsePositiveRateOverPositives FLOAT NOT NULL,
        TruePositiveRateOverPositives FLOAT NOT NULL
    );

    CREATE INDEX IX_BloomMetricsHistory_CreatedAtUtc
        ON dbo.BloomMetricsHistory (CreatedAtUtc);

    CREATE INDEX IX_BloomMetricsHistory_InstanceName_CreatedAtUtc
        ON dbo.BloomMetricsHistory (InstanceName, CreatedAtUtc);

