using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    internal static class BlockHelper
    {

        /// <summary>
        /// 把此块转换为字节数组。
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Block block)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                Consts.formatter.Serialize(ms, block);
                result = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(result, 0, result.Length);
            }

            return result;
        }

        ///// <summary>
        ///// 把字节数组转换为一个块。
        ///// </summary>
        ///// <typeparam name="T">继承自<see cref="Block"/>的类型。</typeparam>
        ///// <param name="bytes"></param>
        ///// <returns></returns>
        //public static T ToBlock<T>(this byte[] bytes) where T : Block
        //{
        //    T result;
        //    using (MemoryStream ms = new MemoryStream(bytes))
        //    {
        //        object obj = formatter.Deserialize(ms);
        //        result = obj as T;
        //    }

        //    return result;
        //}

    }
}
