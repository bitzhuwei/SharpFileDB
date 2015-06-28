using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    static class IStorableHelper
    {
        static IFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// 获取序列化的字节长度。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long SerialzedLength(this IStorable obj)
        {
            long length = 0;
            //byte[] bytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                length = ms.Length;
                //bytes = new byte[ms.Length];
                //ms.Position = 0;
                //ms.Read(bytes, 0, bytes.Length);
            }

            return length;
        }

        const long maxInt32 = int.MaxValue;
        /// <summary>
        /// 获取序列化的字节数组。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(this IStorable obj)
        {
            long length = 0;
            byte[] bytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                length = ms.Length;
                bytes = new byte[length];
                ms.Position = 0;
                if (length < maxInt32)
                {
                    ms.Read(bytes, 0, bytes.Length);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return bytes;
        }
    }
}
