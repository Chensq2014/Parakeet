// See https://aka.ms/new-console-template for more information
using ServiceStack.Redis;

Console.WriteLine("Hello, World!");



//测试6000次读写
var result = Parallel.For(0, 100, (i, state) =>
{
    try
    {
        using RedisClient client = new RedisClient("127.0.0.1");//ok
        //using RedisClient client = new RedisClient("127.0.0.1:6379");//ok
        ///using RedisClient client = new RedisClient("127.0.0.1", 6379, null, 15);
        client.ChangeDb(10);//切换数据库
        client.Set("zxname" + i, i);//i是无序的，因为Parallel并发
        client.Incr("zxname" + i);
        Console.WriteLine("迭代次数：{0},任务ID:{1},线程ID:{2}", i, Task.CurrentId, Thread.CurrentThread.ManagedThreadId);
    }
    catch (Exception e)
    {
        state.Break();
    }
});
Console.WriteLine("是否完成:{0}", result.IsCompleted);
Console.WriteLine("最低迭代:{0}", result.LowestBreakIteration);