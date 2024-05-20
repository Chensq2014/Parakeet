using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    /// <summary>
    /// 双色球：投注号码由6个红色球号码和1个蓝色球号码组成
    /// 红色球号码从01-33中选择，不重复
    /// 蓝色球号码从01-16中选择
    /// 球号码随机的规则，远程获取一个随机数，这个会有较长的时间损耗
    /// 7个球，球是不断变化的，每次变化要花时间等待多线程？---任务是可以并行的，可以使用多线程
    /// </summary>
    public class DoubleBall
    {
        /// <summary>
        /// 获取随机数：
        /// 多线程同时执行结果很高概率相同，是用的当前时间seed,时间相同结果相同,所以扩展一个这样的方法
        /// </summary>
        /// <returns></returns>
        public int GetRamdonNumber()
        {
            return 1;
        }
        public void Begin()
        {
            Task.Run(() =>
            {
                try
                {
                    //this.Invoke();
                }
                catch (Exception)
                {

                    throw;
                }

            });
        }
    }
}
