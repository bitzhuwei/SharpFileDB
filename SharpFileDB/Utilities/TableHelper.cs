using SharpFileDB.Utilities;
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
    public static class TableHelper
    {

        /// <summary>
        /// 把Table的一条记录转换为字节数组。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Table table)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                Consts.formatter.Serialize(ms, table);
                result = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(result, 0, result.Length);
            }

            return result;
        }

        /// <summary>
        /// 把字节数组转换为Table的一条记录。
        /// </summary>
        /// <typeparam name="T">继承自<see cref="Table"/>的类型。</typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T ToTableItem<T>(this byte[] bytes) where T : Table, new()
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
