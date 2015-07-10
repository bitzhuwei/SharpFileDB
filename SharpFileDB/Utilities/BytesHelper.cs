using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
//using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// <see cref="Byte"/>[]类型的辅助类。
    /// </summary>
    public static class BytesHelper
    {

        /// <summary>
        /// 把字节数组转换为一个对象。这个字节数组可能是从一个或多个<see cref="DataBlock.Data"/>读取的。
        /// </summary>
        /// <typeparam name="T">继承自<see cref="Table"/>的类型。</typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ToObject<T>(this byte[] bytes)
        {
            T result;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                object obj = Consts.formatter.Deserialize(ms);
                result = (T)obj;
            }

            Block block = result as Block;
            if (block != null)
            { throw new Exception("Please don't take 'Block' as T!"); }

            return result;
        }

    }
}
