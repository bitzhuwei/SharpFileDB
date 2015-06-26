using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    public static class ObjectLengthHelper
    {
        //static Dictionary<Type, long> lengthDict = new Dictionary<Type, long>();
        static System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        ///// <summary>
        ///// 获取此类型的默认构造函数生成的对象的序列化后的字节数。
        ///// <para>Gets the length of bytes of serialized object that is constructed from default ctor.</para>
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static long GetSerializedLength(this Type type)
        //{
        //    long length;
        //    if (!lengthDict.TryGetValue(type, out length))
        //    {
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            object obj = Activator.CreateInstance(type);
        //            formatter.Serialize(ms, obj);
        //            length = ms.Length;
        //            lengthDict.Add(type, length);
        //        }
        //    }

        //    return length;
        //}

        public static byte[] Serialize(this object obj)
        {
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                bytes = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(bytes, 0, bytes.Length);
            }

            return bytes;
        }
    }
}
