# SnowFlake
雪花算法



简单描述
最高位是符号位，始终为0，不可用。

41位的时间序列，精确到毫秒级，41位的长度可以使用69年。时间位还有一个很重要的作用是可以根据时间进行排序。注意，41位时间截不是存储当前时间的时间截，而是存储时间截的差值（当前时间截 - 开始时间截) 后得到的值，这里的的开始时间截，一般是我们的id生成器开始使用的时间，由我们程序来指定的（如下下面程序SnowFlake类的START_STMP属性）。41位的时间截，可以使用69年，年T = (1L << 41) / (1000L * 60 * 60 * 24 * 365) = 69

10位的机器标识，10位的长度最多支持部署1024个节点。

12位的计数序列号，序列号即一系列的自增id，可以支持同一节点同一毫秒生成多个ID序号，12位的计数序列号支持每个节点每毫秒产生4096个ID序号。

加起来刚好64位，为一个Long型。这个算法很简洁，但依旧是一个很好的ID生成策略。其中，10位器标识符一般是5位IDC+5位machine编号，唯一确定一台机器。




``` c#
class Program
    {
        static void Main(string[] args)
        {
            IdWorker idWorker = new IdWorker(1, 1);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 1000000; i++)
            {
                idWorker.NextId();
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds + "ms");
        }
    }
```
