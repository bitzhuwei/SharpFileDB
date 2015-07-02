using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    internal static class BlockHelper
    {
        static IFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// 把此块写入文件。
        /// </summary>
        /// <param name="block"></param>
        /// <param name="fileStream">数据库文件流。</param>
        public static void Write(this Block block, FileStream fileStream)
        {
            if (fileStream.Length < block.ThisPos)
            {
                fileStream.SetLength(block.ThisPos);
            }
            fileStream.Seek(block.ThisPos, SeekOrigin.Begin);
            formatter.Serialize(fileStream, block);
        }

        /// <summary>
        /// 从文件流中给定的位置读取一个块。
        /// </summary>
        /// <typeparam name="T">块的类型。</typeparam>
        /// <param name="fileStream"></param>
        /// <param name="position">块所在位置。</param>
        /// <returns></returns>
        public static T ReadBlock<T>(this FileStream fileStream, long position) where T : Block
        {
            fileStream.Seek(position, SeekOrigin.Begin);
            object obj = formatter.Deserialize(fileStream);
            T result = obj as T;
            return result;
        }
    }
}
