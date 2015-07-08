using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// <see cref="IComparable"/>类型的辅助类。
    /// </summary>
    public static class IComparableHelper
    {

        /// <summary>
        /// 把此对象转换为字节数组。
        /// <para>最初是为索引的skip list结点的Key添加的此方法。</para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this IComparable key)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                Consts.formatter.Serialize(ms, key);
                result = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(result, 0, result.Length);
            }

            return result;
        }
    }
}
