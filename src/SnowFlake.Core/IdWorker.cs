using System;

namespace SnowFlake
{
    /// <summary>
    /// 分布式ID
    /// </summary>
    public class IdWorker
    {
        public const long Twepoch = 1288834974657L;

        // 工作站占位数
        const int WorkerIdBits = 5;
        // 数据中心占位数
        const int DatacenterIdBits = 5;
        // 微妙并发序列号占位数
        const int SequenceBits = 12;

        // 工作站最大值
        const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        // 数据中心最大值
        const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
        // 微妙并发序列号最大值
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        // 工作站偏移数
        private const int WorkerIdShift = SequenceBits;
        // 数据中心偏移数
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        // 时间戳偏移数
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        // 上一次时间戳
        private long _lastTimestamp = -1L;

        public long WorkerId { get; protected set; } // 工作站ID
        public long DatacenterId { get; protected set; } // 数据中心ID
        public long Sequence { get; internal set; } = 0L; // 并发序列号

        /// <summary>
        /// 实例化ID生成器
        /// </summary>
        /// <param name="workerId">工作站ID</param>
        /// <param name="datacenterId">数据中心ID</param>
        /// <param name="sequence">并发序列号</param>
        public IdWorker(long workerId, long datacenterId, long sequence = 0L)
        {
            WorkerId = workerId;
            DatacenterId = datacenterId;
            Sequence = sequence;

            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException(String.Format("工作站ID不能大于 {0} 或小于 0", MaxWorkerId));
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException(String.Format("数据中心ID不能大于 {0} 或小于 0", MaxDatacenterId));
            }
        }

        readonly object _lock = new Object();

        /// <summary>
        /// 获取下一个ID
        /// </summary>
        /// <returns></returns>
        public virtual long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                {
                    throw new Exception(String.Format("时钟回流，拒绝生成。", _lastTimestamp - timestamp));
                }

                if (_lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & SequenceMask;
                    if (Sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    Sequence = 0;
                }

                _lastTimestamp = timestamp;
                var id = ((timestamp - Twepoch) << TimestampLeftShift) | (DatacenterId << DatacenterIdShift) | (WorkerId << WorkerIdShift) | Sequence;

                return id;
            }
        }

        /// <summary>
        /// 获取下一个时间戳
        /// </summary>
        /// <param name="lastTimestamp">上一次时间戳</param>
        /// <returns></returns>
        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        /// <summary>
        /// 当前时间戳
        /// </summary>
        /// <returns></returns>
        protected virtual long TimeGen() => System.CurrentTimeMillis();
    }
}