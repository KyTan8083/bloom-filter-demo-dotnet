# Bloom Filter Demo (.NET)

A production-style C# demo showing how to use a Bloom Filter with cache, track false positives, print real-time metrics, and persist metrics snapshots into SQL Server using a background service.
一个生产级示例，展示如何在 C# 中使用 Bloom Filter，结合缓存、自动统计误判（false positive）、实时指标输出，并通过后台服务将指标持久化到数据库。

---

## 🚀 Features | 功能

- Bloom Filter abstraction  
  Bloom Filter 抽象设计
- In-memory implementation (ready for Redis)  
  内存实现（可扩展为 Redis）
- Automatic false positive tracking  
  自动统计误判率
- Real-time metrics logging  
  实时指标日志输出
- Metrics API endpoint  
  提供 metrics API
- Background metrics persistence (Dapper + SQL)  
  后台服务定时写入数据库（Dapper）
- Console client for API testing & load testing  
  Console 测试工具（支持压测）

---

## 🏗️ Architecture | 架构说明
Client → API → Bloom Filter → Authoritative Store
                         ↓
                     Metrics
                         ↓
         Background Persist Service → Database

## SQL Script | 数据库脚本
/sql/create_bloom_metrics_history.sql

打开 SQL Server / SSMS
选择数据库
执行该 SQL 文件


Configuration | 配置
appsettings.json（示例）

{
  "ConnectionStrings": {
    "MetricsDb": "Server=localhost;Database=BloomMetricsDb;..."
  },
  "BloomMetricsPersist": {
    "Enabled": true,
    "IntervalSeconds": 60,
    "ForcePersistIntervalMinutes": 30
  }
}


## Run Project | 运行方式
还原依赖
dotnet restore

启动 API
dotnet run --project BloomFilterDemo

启动测试工具
dotnet run --project BloomFilterDemo.TestClient