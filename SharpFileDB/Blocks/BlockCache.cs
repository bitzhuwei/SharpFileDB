using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 缓存从数据库读取的<see cref="Block"/>和通过<code>new Block()</code>新建的<see cref="Block"/>。
    /// </summary>
    public static class BlockCache
    {

        /// <summary>
        /// <see cref="BlockCache.sunkBlocksInMomery"/>能存储的<see cref="Block"/>数目的最大值。如果达到最大值，就会清空<see cref="BlockCache.sunkBlocksInMomery"/>。
        /// </summary>
        public static long MaxSunkCountInMemory = 10001;

        /// <summary>
        /// 所有内存中尚未分配其在数据库文件中的位置的<see cref="Block"/>对象。其<see cref="Block.ThisPos"/>应为0。
        /// </summary>
        private static readonly List<Block> floatingBlocksInMemory = new List<Block>();

        /// <summary>
        /// 内存中所有已分配了其在数据库文件中的位置的<see cref="Block"/>对象，根据其位置<see cref="Block.ThisPos"/>放入字典。
        /// </summary>
        private static readonly Dictionary<long, Block> sunkBlocksInMomery = new Dictionary<long, Block>();


        /// <summary>
        /// <code>new Block()</code>时要加入floating列表。
        /// </summary>
        /// <param name="block"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddFloatingBlock(Block block)
        {
            BlockCache.floatingBlocksInMemory.Add(block);
        }

        /// <summary>
        /// 已写入数据库的<see cref="Block"/>应从floating列表中移除。
        /// </summary>
        /// <param name="block"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryRemoveFloatingBlock(Block block)
        {
            if (BlockCache.floatingBlocksInMemory.Contains(block))
            {
                BlockCache.floatingBlocksInMemory.Remove(block);
            }
        }

        /// <summary>
        /// 尝试获取已从数据库中反序列化得到的<see cref="Block"/>。
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Block TryGetSunkBlock(long position)
        {
            Block block = null;
            BlockCache.sunkBlocksInMomery.TryGetValue(position, out block);
            return block;
        }
        /// <summary>
        /// 已写入数据库的<see cref="Block"/>应加入Sunk字典。
        /// </summary>
        /// <param name="block"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddSunkBlock(Block block)
        {
            if (BlockCache.sunkBlocksInMomery.ContainsKey(block.ThisPos))
            {
                Block exsistsBlock = BlockCache.sunkBlocksInMomery[block.ThisPos];
                if (block != exsistsBlock)
                { throw new Exception("Too blocks take the same position!"); }
            }
            else
            {
                BlockCache.sunkBlocksInMomery.Add(block.ThisPos, block);
            }

            if (BlockCache.sunkBlocksInMomery.LongCount() >= BlockCache.MaxSunkCountInMemory)
            {
                BlockCache.sunkBlocksInMomery.Clear();
                GC.Collect();
            }
        }

        /// <summary>
        /// 从block从sunk字典移除指定的sunk过的block。
        /// </summary>
        /// <param name="block"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryRemoveSunkBlock(Block block)
        {
            if (BlockCache.sunkBlocksInMomery.ContainsKey(block.ThisPos))
            {
                BlockCache.sunkBlocksInMomery.Remove(block.ThisPos);
            }
        }

    }
}
