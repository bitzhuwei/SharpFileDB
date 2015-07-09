using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
//using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// IndexBlock充当了<see cref="SkipListNodeBlock"/>的skip list角色。
    /// </summary>
    public static partial class IndexBlockHelper
    {

        /// <summary>
        /// 查找小于给定的<paramref name="key"/>的各个结点列中的最接近<paramref name="key"/>的结点列。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="indexBlock">充当skip list。</param>
        /// <param name="db"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SkipListNodeBlock[] FindRightMostNodes(IComparable key, IndexBlock indexBlock, FileDBContext db)
        {
            FileStream fs = db.fileStream;
            int maxLevel = db.headerBlock.MaxLevelOfSkipList;

            SkipListNodeBlock[] rightNodes = new SkipListNodeBlock[maxLevel];

            // Start at the top list header node
            SkipListNodeBlock currentNode = indexBlock.SkipListHeadNodes[indexBlock.CurrentLevel];

            for (int i = indexBlock.CurrentLevel; i >= 0; i--)
            {
                IComparable rightKey = db.GetRightObjKey(db.fileStream, indexBlock, currentNode);
                while ((currentNode.RightObj != indexBlock.SkipListTailNode) && (rightKey.CompareTo(key) < 0))
                {
                    currentNode = currentNode.RightObj;

                    rightKey = db.GetRightObjKey(db.fileStream, indexBlock, currentNode);
                }

                rightNodes[i] = currentNode;

                if (i > 0)
                {
                    currentNode.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.DownObj);
                    currentNode = currentNode.DownObj;
                }
            }

            return rightNodes;
        }

    }
}
