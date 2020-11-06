# SnowFlake
雪花算法


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
