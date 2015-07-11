using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using SharpFileDB.Blocks;
using SharpFileDB.Utilities;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SharpFileDB
{
    /// <summary>
    /// 单文件数据库上下文，代表一个单文件数据库。SharpFileDB的核心类型。
    /// </summary>
    public partial class FileDBContext : IDisposable
    {

        #region 属性/字段

        /// <summary>
        /// 数据库文件据对路径。 
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        /// 临时日志文件名（全名）。
        /// </summary>
        internal string JournalFileName { get { return string.Format("{0}journal", this.Fullname); } }

        /// <summary>
        /// 用于读写数据库文件的文件流。
        /// </summary>
        internal FileStream fileStream;

        /// <summary>
        /// 数据库头部。
        /// </summary>
        internal DBHeaderBlock headerBlock;

        /// <summary>
        /// 数据库表的头结点。
        /// </summary>
        internal TableBlock tableBlockHead;

        /// <summary>
        /// 事务。
        /// </summary>
        internal Transaction transaction;

        /// <summary>
        /// 存储数据库表的字典。不含表的头结点。
        /// </summary>
        internal Dictionary<Type, TableBlock> tableBlockDict = new Dictionary<Type, TableBlock>();

        /// <summary>
        /// 数据库索引的字典。不含索引的头结点。
        /// </summary>
        internal Dictionary<Type, Dictionary<string, IndexBlock>> tableIndexBlockDict = new Dictionary<Type, Dictionary<string, IndexBlock>>();

        #endregion 属性/字段

        /// <summary>
        /// 查找具有指定的key值的结点列的最下方的结点。
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="key"></param>
        /// <param name="indexBlock"></param>
        /// <returns></returns>
        //private SkipListNodeBlock FindSkipListNode(FileStream fileStream, IComparable key, IndexBlock indexBlock)
        private SkipListNodeBlock FindSkipListNode(FileStream fileStream, IndexBlock indexBlock, IComparable key)
        {
            // Start at the top list header node
            SkipListNodeBlock currentNode = indexBlock.SkipListHeadNodes[indexBlock.CurrentLevel];

            IComparable rightKey = null;

            while (true)
            {
                rightKey = GetRightObjKey(fileStream, indexBlock, currentNode);

                while ((currentNode.RightObj != indexBlock.SkipListTailNode) && (rightKey.CompareTo(key) < 0))
                {
                    currentNode = currentNode.RightObj;

                    rightKey = GetRightObjKey(fileStream, indexBlock, currentNode);
                }

                // Check if there is a next level, and if there is move down.
                if (currentNode.DownPos == 0)
                {
                    break;
                }
                else
                {
                    currentNode.TryLoadProperties(fileStream, SkipListNodeBlockLoadOptions.DownObj);
                    currentNode = currentNode.DownObj;
                }
            }

            // Do one final comparison to see if the key to the right equals this key.
            // If it doesn't match, it would be bigger than this key.
            if (rightKey.CompareTo(key) == 0)
            {
                return currentNode.RightObj;
            }
            else
            { return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="indexBlock"></param>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining)]
        internal IComparable GetRightObjKey(FileStream fileStream, IndexBlock indexBlock, SkipListNodeBlock currentNode)
        {
            IComparable rightKey = null;
            if (currentNode.RightPos == indexBlock.SkipListTailNodePos)
            { currentNode.RightObj = indexBlock.SkipListTailNode; }
            else
            {
                currentNode.TryLoadProperties(fileStream, SkipListNodeBlockLoadOptions.RightObj);
                currentNode.RightObj.TryLoadProperties(fileStream, SkipListNodeBlockLoadOptions.Key);
                rightKey = currentNode.RightObj.Key.GetObject<IComparable>(fileStream);
            }
            return rightKey;
        }


        #region IDisposable Members

        /// <summary>
        /// Internal variable which checks if Dispose has already been called
        /// </summary>
        private Boolean disposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(Boolean disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Managed cleanup code here, while managed refs still valid
            }
            // Unmanaged cleanup code here
            this.fileStream.Close();
            this.fileStream.Dispose();

            disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the private Dispose(bool) helper and indicate 
            // that we are explicitly disposing
            this.Dispose(true);

            // Tell the garbage collector that the object doesn't require any
            // cleanup when collected since Dispose was called explicitly.
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
