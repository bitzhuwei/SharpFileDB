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
        /// 把Table的一条记录转换为字节数组。这个字节数组应该保存到Data页。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Table table)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                Consts.formatter.Serialize(ms, table);
                if (ms.Length > (long)int.MaxValue)// RULE: 一条记录序列化后最长不能超过int.MaxValue个字节。
                { throw new Exception(string.Format("Toooo long is the [{0}]", table)); }
                result = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(result, 0, result.Length);
            }

            return result;
        }

    }
}
