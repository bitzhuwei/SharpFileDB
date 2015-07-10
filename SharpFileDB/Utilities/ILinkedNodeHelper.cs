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

    }
}
