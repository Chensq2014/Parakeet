using IdGen;
using System.Linq;

namespace Parakeet.Net.Helper
{
    /// <summary>
    /// 雪花算法生成长整型Id
    /// </summary>
    public static class SnowflakeIdGenHelper
    {
        public static long NextId()
        {
            var generator = new IdGenerator(0);
            var id = generator.CreateId();
            return id;
        }

        public static long[] NextIdArray(int size)
        {
            var generator = new IdGenerator(0);
            var ids = generator.Take(size);
            return ids.ToArray();
        }
    }
}
