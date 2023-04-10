using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parakeet.Net.Converter.Server
{
    class BitContainer
    {
        private readonly ConcurrentDictionary<int, char> _dic = new ConcurrentDictionary<int, char>();

        internal void Add(int bit, char charValue)
        {
            _dic.TryAdd(bit, charValue);
        }

        /// <summary>
        /// 重写ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < _dic.Keys.Max() + 1; i++)
            {
                if (_dic.Keys.Contains(i))
                {
                    sb.Append(_dic[i]);
                }
                else
                {
                    sb.Append('0');
                }
                if ((i + 1) % 4 == 0)
                {
                    sb.Append(' ');
                }
            }

            var charArray = sb.ToString().ToCharArray().Reverse().ToArray();
            return new string(charArray);
        }
    }
}
