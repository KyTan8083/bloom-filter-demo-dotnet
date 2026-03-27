using System.Threading;
using BloomFilterDemo.Models;

namespace BloomFilterDemo.Services
{
    //Thread-safe metrics collector.
    //很多 request 会同时进来更新统计数字，所以这个 class 必须是 thread-safe。
    //Interlocked.Increment 是什么: 以 thread-safe 的方式，把这个数字加 1. 不会发生“两个 thread 都读到旧值然后覆盖掉彼此结果”的问题。

    public sealed class BloomMetrics
    {

        private long _totalChecks;
        private long _bloomNegativeCount;
        private long _bloomPositiveCount;
        private long _truePositiveCount;
        private long _falsePositiveCount;
        private long _inserts;

        //记录 "不存在" 
        public void RecordBloomNegative()
        {
            Interlocked.Increment(ref _totalChecks); //所以总检查数 +1
            Interlocked.Increment(ref _bloomNegativeCount); //bloom negative 数 +1
        }

        //记录 "可能存在"
        public void RecordBloomPositive(bool actualExists)
        {
            Interlocked.Increment(ref _totalChecks);
            Interlocked.Increment(ref _bloomPositiveCount);

            if (actualExists)
            {
                Interlocked.Increment(ref _truePositiveCount);
            }
            else
            {
                Interlocked.Increment(ref _falsePositiveCount);
            }
        }

        //记录插入次数
        public void RecordInsert()
        {
            Interlocked.Increment(ref _inserts);
        }

        //把当前 counters 复制一份出来给别人看
        public BloomMetricsSnapshot Snapshot()
        {
            //读也可能有并发问题
            //但如果多个 thread 正在更新，而另一个 thread 正在读取，还是要尽量保证读取方式安全、稳定
            //Interlocked.Read

            return new BloomMetricsSnapshot
            {
                TotalChecks = Interlocked.Read(ref _totalChecks),
                BloomNegativeCount = Interlocked.Read(ref _bloomNegativeCount),
                BloomPositiveCount = Interlocked.Read(ref _bloomPositiveCount),
                TruePositiveCount = Interlocked.Read(ref _truePositiveCount),
                FalsePositiveCount = Interlocked.Read(ref _falsePositiveCount),
                Inserts = Interlocked.Read(ref _inserts)
            };
        }
    }
}
