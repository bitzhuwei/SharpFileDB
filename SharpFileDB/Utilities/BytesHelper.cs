using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    public static class BytesHelper
    {

        /// <summary>
        /// 把字节数组转换为Table的一条记录。这个字节数组应该是从Data页读取的。
        /// </summary>
        /// <typeparam name="T">继承自<see cref="Table"/>的类型。</typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] bytes)// where T : classTable, new()
        {
            T result;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                object obj = Consts.formatter.Deserialize(ms);
                result = (T)obj;
            }

            return result;
        }

        /// <summary>
        /// 把字节数组转换为一个块。
        /// </summary>
        /// <typeparam name="T">继承自<see cref="Block"/>的类型。</typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T ToBlock<T>(this byte[] bytes) where T : Block
        {
            T result;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                object obj = Consts.formatter.Deserialize(ms);
                result = obj as T;
            }

            return result;
        }

    }
}
