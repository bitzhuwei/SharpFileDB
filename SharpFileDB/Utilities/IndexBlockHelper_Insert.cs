using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// IndexBlock充当了skip list node block的skip list角色。
    /// </summary>
    public static partial class IndexBlockHelper
    {

        static readonly Random rand = new Random();

        /// <summary>
        /// 为给定索引安排新的记录。
        /// </summary>
        /// <param name="indexBlock">索引。</param>
        /// <param name="item">新插入的记录。</param>
        /// <param name="dataBlocksForValue">为新插入的记录安排好的数据块。</param>
        /// <param name="db">数据库上下文。</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Insert(this IndexBlock indexBlock, Table item, DataBlock[] dataBlocksForValue, FileDBContext db)
        {
            Type type = item.GetType();
            PropertyInfo property = type.GetProperty(indexBlock.BindMember);
            //if(members.Length != 1)
            //{
            //    throw new Exception(string.Format("[{0}] items named with index key's name [{1}]", members.Length, indexBlock.BindMember)); 
            //}
            TableIndexAttribute attr = property.GetCustomAttribute<TableIndexAttribute>();
            if (attr == null)
            { throw new Exception(string.Format("No TableIndexAttribute binded!")); }

            // 准备Key。
            var key = property.GetValue(item) as IComparable;
            byte[] keyBytes = key.ToBytes();
            if (keyBytes.Length > Consts.maxDataBytes)
            { throw new Exception(string.Format("Toooo long is the key [{0}]", key)); }
            DataBlock dataBlockForKey = new DataBlock() { ObjectLength = keyBytes.Length, Data = keyBytes, };

            SkipListNodeBlock[] rightNodes = FindRightMostNodes(key, indexBlock, db);

            //// Check if the item allready exists in the list.  If it does, throw an exception -
            //// we will not allow duplicate items here.
            FileStream fs = db.fileStream;
            int maxLevel = db.headerBlock.MaxLevelOfSkipList;

            IComparable rightKey = null;
            if (rightNodes[0].RightPos != indexBlock.SkipListTailNode.ThisPos)
            {
                rightNodes[0].TryLoadProperties(fs, LoadOptions.RightObj);
                rightNodes[0].RightObj.TryLoadProperties(fs, LoadOptions.Key);
                rightKey = rightNodes[0].RightObj.Key.GetObject<IComparable>(fs);
            }
            else
            { rightNodes[0].RightObj = indexBlock.SkipListTailNode; }


            if ((rightNodes[0].RightObj != indexBlock.SkipListTailNode)
                && (rightKey.CompareTo(key) == 0))// key相等，说明Value相同。此处不再使用NGenerics的Comparer<TKey>.Default这种可指定外部比较工具的模式，是因为那会由于忘记编写合适的比较工具而带来隐藏地很深的bug。
            {
                throw new Exception("Item Already In List");
            }
            else
            {
                int newLevel = PickRandomLevel(db, indexBlock);

                if (newLevel > indexBlock.CurrentLevel)
                {
                    for (int i = indexBlock.CurrentLevel + 1; i <= newLevel; i++)
                    {
                        rightNodes[i] = indexBlock.SkipListHeadNodes[i];
                    }

                    indexBlock.CurrentLevel = newLevel;
                }

                List<SkipListNodeBlock> nodeList = new List<SkipListNodeBlock>();
                SkipListNodeBlock newNode = new SkipListNodeBlock() { Key = dataBlockForKey, Value = dataBlocksForValue, };
                nodeList.Add(newNode);

                // Insert the item in the first level
                newNode.RightObj = rightNodes[0].RightObj;
                rightNodes[0].RightObj = newNode;

                // And now insert the node in the rest of the levels, making sure
                // to update the the links
                SkipListNodeBlock previousNode = null;
                for (int i = 1; i <= indexBlock.CurrentLevel; i++)
                {
                    previousNode = newNode;
                    newNode = new SkipListNodeBlock() { Key = dataBlockForKey, Value = dataBlocksForValue, };
                    nodeList.Add(newNode);

                    newNode.RightObj = rightNodes[i].RightObj;
                    rightNodes[i].RightObj = newNode;

                    newNode.DownObj = previousNode;
                }

                for (int i = nodeList.Count - 1; i >= 0; i--)
                { db.transaction.Add(rightNodes[i]); }// 加入事务，准备写入数据库。

                for (int i = nodeList.Count - 1; i >= 0; i--)
                { db.transaction.Add(nodeList[i]); }// 加入事务，准备写入数据库。

                db.transaction.Add(dataBlockForKey);// 加入事务，准备写入数据库。
            }

            //itemsCount++;// 有的在内存，有的在文件，因此itemsCount不好使了。
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int PickRandomLevel(FileDBContext db, IndexBlock indexBlock)
        {
            int randomLevel = 0;

            int maxLevel = db.headerBlock.MaxLevelOfSkipList;
            double probability = db.headerBlock.ProbabilityOfSkipList;

            while ((rand.NextDouble() < probability) && (randomLevel <= indexBlock.CurrentLevel + 1) && (randomLevel < maxLevel))
            {
                randomLevel++;
            }

            return randomLevel;
        }

    }
}
