using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
//using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// <see cref="FileStream"/>类型的辅助类。
    /// </summary>
    public static class FileStreamHelper
    {

        /// <summary>
        /// 把此块写入文件。
        /// </summary>
        /// <param name="fileStream">数据库文件流。</param>
        /// <param name="block"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteBlock(this FileStream fileStream, Block block)
        {
            if (fileStream.Length < block.ThisPos)
            {
                fileStream.SetLength(block.ThisPos);
            }
            fileStream.Seek(block.ThisPos, SeekOrigin.Begin);
            Consts.formatter.Serialize(fileStream, block);

            BlockCache.TryRemoveFloatingBlock(block);// 如果是new Block()，需要在此时从floating列表移除
            BlockCache.AddSunkBlock(block);// 如果是new Block()，需要在此时加入sunk字典
        }

        /// <summary>
        /// 从文件流中给定的位置读取一个块。
        /// </summary>
        /// <typeparam name="T">块的类型。</typeparam>
        /// <param name="fileStream"></param>
        /// <param name="position">块所在位置。</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadBlock<T>(this FileStream fileStream, long position) where T : Block
        {
            T block = null;

            Block b = BlockCache.TryGetSunkBlock(position);// 如果已经从数据库读出来过，就不应再读了。
            if(b != null)
            {
                block = b as T;
                if (block == null)
                { throw new Exception("Two types of Block exists in the same position!"); }
            }
            else
            {
                fileStream.Seek(position, SeekOrigin.Begin);
                object obj = Consts.formatter.Deserialize(fileStream);
                block = obj as T;
                block.ThisPos = position;

                BlockCache.AddSunkBlock(block);
            }

            return block ;
        }

        /// <summary>
        /// 从文件流中给定的位置读取一个由块为结点组成的文件链表。
        /// </summary>
        /// <typeparam name="T">块的类型。</typeparam>
        /// <param name="fileStream"></param>
        /// <param name="position">第一个块的位置。</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] ReadBlocks<T>(this FileStream fileStream, long position) where T : Block, ILinkedNode<T>
        {
            List<T> list = new List<T>();
            T item = ReadBlock<T>(fileStream, position);
            list.Add(item);

            while (item.NextPos != 0)
            {
                item.NextObj = ReadBlock<T>(fileStream, item.NextPos);
                list.Add(item.NextObj);
                item = item.NextObj;
            }

            return list.ToArray();
        }

    }
}
