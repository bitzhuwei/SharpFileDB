using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// IndexBlock充当了skip list node block的skip list角色。
    /// </summary>
    public static class IndexBlockHelper
    {
        static readonly Random rand = new Random();

        /// <summary>
        /// 为给定索引安排新的记录。
        /// </summary>
        /// <param name="indexBlock">索引。</param>
        /// <param name="item">新插入的记录。</param>
        /// <param name="dataBlocks"></param>
        /// <param name="db">数据库上下文。</param>
        public static void Add(this IndexBlock indexBlock, Table item, DataBlock[] dataBlocks, FileDBContext db)
        {

            throw new NotImplementedException();
        }

        ///// <summary>
        ///// 为给定索引安排新的记录。
        ///// </summary>
        ///// <param name="indexBlock">索引。</param>
        ///// <param name="item">新插入的记录。</param>
        ///// <param name="db">数据库上下文。</param>
        //public static void Add(this IndexBlock indexBlock, Table item, FileDBContext db)
        //{
        //    Type type = item.GetType();
        //    PropertyInfo property = type.GetProperty(indexBlock.BindMember);
        //    //if(members.Length !=1)
        //    //{
        //    //    throw new Exception(string.Format("[{0}] items named with index key's name [{1}]", members.Length, indexBlock.BindMember)); 
        //    //}
        //    TableIndexAttribute attr = property.GetCustomAttribute<TableIndexAttribute>();
        //    if (attr == null)
        //    {
        //        throw new Exception(string.Format("No TableIndexAttribute binded!"));
        //    }

        //    var key = property.GetValue(item) as IComparable;
        //    SkipListNodeBlock[] rightNodes = FindRightMostNodes(key, indexBlock, db);

        //    //// Check if the item allready exists in the list.  If it does, throw an exception -
        //    //// we will not allow duplicate items here.
        //    FileStream fs = db.fileStream;

        //    int maxLevel = db.headerBlock.MaxLevelOfSkipList;

        //    SkipListNodeBlock right = fs.ReadObject<SkipListNodeBlock>(rightNodes[0].RightPos);
        //    IComparable rightKey = fs.ReadObject<IComparable>(right.KeyPos);
        //    if ((rightNodes[0].RightPos != 0) && (rightKey.CompareTo(key) == 0))
        //    {
        //        throw new Exception("Item Already In List");
        //    }
        //    else
        //    {
        //        int newLevel = PickRandomLevel(db, indexBlock);

        //        if (newLevel > indexBlock.CurrentLevel)
        //        {
        //            for (int i = indexBlock.CurrentLevel + 1; i <= newLevel; i++)
        //            {
        //                rightNodes[i] = indexBlock.SkipListHeadNodes[i];
        //            }

        //            indexBlock.CurrentLevel = newLevel;
        //        }

        //        SkipListNodeBlock previousNode = null;
        //        SkipListNodeBlock newNode = new SkipListNodeBlock();// key, value暂时没有指定。

        //        // Insert the item in the first level
        //        newNode.Right = rightNodes[0].Right;
        //        rightNodes[0].Right = newNode;

        //        // And now insert the node in the rest of the levels, making sure
        //        // to update the the links
        //        for (int i = 1; i <= currentLevel; i++)
        //        {
        //            previousNode = newNode;
        //            newNode = new SkipListNode<TKey, TValue>(key, value);

        //            newNode.Right = rightNodes[i].Right;
        //            rightNodes[i].Right = newNode;

        //            newNode.Down = previousNode;
        //        }
        //    }

        //    //itemsCount++;
        //}

        //private static int PickRandomLevel(FileDBContext db, IndexBlock indexBlock)
        //{
        //    int randomLevel = 0;

        //    int maxLevel = db.headerBlock.MaxLevelOfSkipList;
        //    double probability = db.headerBlock.ProbabilityOfSkipList;

        //    while ((rand.NextDouble() < probability) && (randomLevel <= indexBlock.CurrentLevel + 1) && (randomLevel < maxLevel))
        //    {
        //        randomLevel++;
        //    }

        //    return randomLevel;
        //}

        //private static SkipListNodeBlock[] FindRightMostNodes(IComparable key, IndexBlock indexBlock, FileDBContext db)
        //{
        //    FileStream fs = db.fileStream;

        //    int maxLevel = db.headerBlock.MaxLevelOfSkipList;

        //    SkipListNodeBlock[] rightNodes = new SkipListNodeBlock[maxLevel];

        //    // Start at the top list header node
        //    //SkipListNode<TKey, TValue> currentNode = headNodes[indexBlock.CurrentLevel];
        //    SkipListNodeBlock currentNode = indexBlock.SkipListHeadNodes[indexBlock.CurrentLevel];

        //    //for (int i = indexBlock.CurrentLevel; i >= 0; i--)
        //    //{
        //    //    while ((currentNode.Right != tail) && (comparerToUse.Compare(currentNode.Right.Key, key) < 0))
        //    //    {
        //    //        currentNode = currentNode.Right;
        //    //    }

        //    //    // Store this node - the new node will be to the right of it.
        //    //    rightNodes[i] = currentNode;

        //    //    // Check if there is a next level, and if there is move down.
        //    //    if (i > 0)
        //    //    {
        //    //        currentNode = currentNode.Down;
        //    //    }
        //    //}
        //    //return rightNodes;
        //    for (int i = indexBlock.CurrentLevel; i >= 0; i--)
        //    {
        //        while ((currentNode.RightPos != 0))
        //        {
        //            SkipListNodeBlock right = fs.ReadObject<SkipListNodeBlock>(currentNode.RightPos);
        //            IComparable rightKey = fs.ReadObject<IComparable>(right.KeyPos);
        //            if (rightKey.CompareTo(key) < 0)
        //            { currentNode = right; }
        //            else
        //            { break; }
        //        }
        //        rightNodes[i] = currentNode;
        //        if(i>0)
        //        {
        //            SkipListNodeBlock down = fs.ReadObject<SkipListNodeBlock>(currentNode.DownPos);
        //            currentNode = down;
        //        }
        //    }
        //    return rightNodes;
        //}
    }
}
