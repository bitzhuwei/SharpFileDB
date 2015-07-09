using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
//using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// <see cref="ILinkedNode&lt;T&gt;"/>类型的辅助类。
    /// </summary>
    public static class ILinkedNodeHelper
    {
        /// <summary>
        /// 如果尚未加载<see cref="ILinkedNode&lt;T&gt;.NextObj"/>，就用<see cref="ILinkedNode&lt;T&gt;.NextPos"/>加载之。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="fileStream"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryLoadNextObj<T>(this T node, FileStream fileStream) where T : Block, ILinkedNode<T>
        {
            if (node.NextPos != 0 && node.NextObj == null)
            {
                node.NextObj = fileStream.ReadBlock<T>(node.NextPos);
            }
        }

        //public static void TryLoadProperties(this IndexBlock block, FileStream fileStream, IndexBlockLoadOptions options)
        //{
        //    if (block.NextPos != 0 && block.NextObj == null && (options & IndexBlockLoadOptions.NextObj) == IndexBlockLoadOptions.NextObj)
        //    {
        //        block.NextObj = fileStream.ReadBlock<IndexBlock>(block.NextPos);
        //    }

        //    if(block.SkipListHeadNodePos!=0&&block.SkipListHeadNodes==null&&(options & IndexBlockLoadOptions.SkipListHeadNodes) == IndexBlockLoadOptions.SkipListHeadNodes)
        //    {
        //        block.sk
        //    }
        //}

        /// <summary>
        /// 如果尚未加载<see cref="SkipListNodeBlock"/>的各项属性对象，就尝试加载之。
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fileStream"></param>
        /// <param name="options">指定需要加载的属性对象，可组合使用。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryLoadProperties(this SkipListNodeBlock node, FileStream fileStream, SkipListNodeBlockLoadOptions options = SkipListNodeBlockLoadOptions.RightObj |SkipListNodeBlockLoadOptions.DownObj | SkipListNodeBlockLoadOptions.Key | SkipListNodeBlockLoadOptions.Value)// where T : SkipListNodeBlock//, ISkipListNode<T>
        {
            if (node.RightPos != 0 && node.RightObj == null && (options & SkipListNodeBlockLoadOptions.RightObj) == SkipListNodeBlockLoadOptions.RightObj)
            {
                node.RightObj = fileStream.ReadBlock<SkipListNodeBlock>(node.RightPos);
            }

            if (node.DownPos != 0 && node.DownObj == null && (options & SkipListNodeBlockLoadOptions.DownObj) == SkipListNodeBlockLoadOptions.DownObj)
            {
                node.DownObj = fileStream.ReadBlock<SkipListNodeBlock>(node.DownPos);
            }

            if (node.KeyPos != 0 && node.Key == null && (options & SkipListNodeBlockLoadOptions.Key) == SkipListNodeBlockLoadOptions.Key)
            {
                node.Key = fileStream.ReadBlock<DataBlock>(node.KeyPos);
            }

            if (node.ValuePos != 0 && node.Value == null && (options & SkipListNodeBlockLoadOptions.Value) == SkipListNodeBlockLoadOptions.Value)
            {
                node.Value = fileStream.ReadBlocks<DataBlock>(node.ValuePos);
            }
        }

    }

    ///// <summary>
    ///// 标识要加载的属性，可以组合。
    ///// </summary>
    //[Flags]
    //public enum IndexBlockLoadOptions
    //{

    //    /// <summary>
    //    /// 应加载NextObj
    //    /// </summary>
    //    NextObj = 1,

    //    /// <summary>
    //    /// 应加载SkipListHeadNodes
    //    /// </summary>
    //    SkipListHeadNodes = 2,

    //    /// <summary>
    //    /// 应加载Key
    //    /// </summary>
    //    Key = 4,

    //    /// <summary>
    //    /// 应加载Value
    //    /// </summary>
    //    Value = 8,
    //}

    /// <summary>
    /// 标识要加载的属性，可以组合。
    /// </summary>
    [Flags]
    public enum SkipListNodeBlockLoadOptions
    {

        /// <summary>
        /// 应加载RightObj
        /// </summary>
        RightObj = 1,

        /// <summary>
        /// 应加载DownObj
        /// </summary>
        DownObj = 2,

        /// <summary>
        /// 应加载Key
        /// </summary>
        Key = 4,

        /// <summary>
        /// 应加载Value
        /// </summary>
        Value = 8,
    }
}
