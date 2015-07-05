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

        public static AllocPageTypes BelongedPageType(this Block block)
        {
            AllocPageTypes pageType;
            Type type = block.GetType();
            if (type == typeof(DataBlock))
            { pageType = AllocPageTypes.Data; }
            else if (type == typeof(SkipListNodeBlock))
            { pageType = AllocPageTypes.SkipListNode; }
            else if (type == typeof(IndexBlock))
            { pageType = AllocPageTypes.Index; }
            else if (type == typeof(TableBlock))
            { pageType = AllocPageTypes.Table; }
            else
            { throw new Exception("Wrong Block Type!"); }

            return pageType;
        }
    }
}
