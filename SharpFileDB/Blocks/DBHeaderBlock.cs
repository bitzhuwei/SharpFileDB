using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 数据库文件的头部。应该放在数据库文件的第一个页。
    /// </summary>
    [Serializable]
    public class DBHeaderBlock : Block//, IUpdatable
    {
#if DEBUG
        private long blockCount;

        /// <summary>
        /// 到目前为止通过new XxxBlock()创建的Block总数。
        /// </summary>

        public long BlockCount
        {
            get { return blockCount; }
            set
            {
                if (blockCount != value)
                {
                    blockCount = value;
                    this.IsDirty = true;
                }
            }
        }
#endif

        private long firstTablePagePos;
        private long firstIndexPagePos;
        private long firstSkipListNodePagePos;
        private long firstDataPagePos;
        private long firstEmptyPagePos;

        private int maxLevelOfSkipList;
        private double probabilityOfSkipList;
        private long maxSunkCountInMemory;
        private TimeSpan lockTimeout;

        /// <summary>
        /// 第一个存储<see cref="TableBlock"/>的页的位置。
        /// </summary>
        public long FirstTablePagePos
        {
            get { return firstTablePagePos; }
            set
            {
                if (firstTablePagePos != value)
                {
                    firstTablePagePos = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 第一个存储<see cref="IndexBlock"/>的页的位置。
        /// </summary>
        public long FirstIndexPagePos
        {
            get { return firstIndexPagePos; }
            set
            {
                if (firstIndexPagePos != value)
                {
                    firstIndexPagePos = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 第一个存储<see cref="SkipListNodeBlock"/>的页的位置。
        /// </summary>
        public long FirstSkipListNodePagePos
        {
            get { return firstSkipListNodePagePos; }
            set
            {
                if (firstSkipListNodePagePos != value)
                {
                    firstSkipListNodePagePos = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 第一个存储<see cref="DataBlock"/>的页的位置。
        /// </summary>
        public long FirstDataPagePos
        {
            get { return firstDataPagePos; }
            set
            {
                if (firstDataPagePos != value)
                {
                    firstDataPagePos = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 第一个存储空白页的位置。
        /// <para>当数据库删除某些内容后，可能会出现一些页不再被占用，此时它们就成为空白页。</para>
        /// </summary>
        public long FirstEmptyPagePos
        {
            get { return firstEmptyPagePos; }
            set
            {
                if (firstEmptyPagePos != value)
                {
                    firstEmptyPagePos = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 索引使用的skip list的max level参数。
        /// </summary>
        public int MaxLevelOfSkipList
        {
            get { return maxLevelOfSkipList; }
            set
            {
                if (maxLevelOfSkipList != value)
                {
                    maxLevelOfSkipList = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 索引使用的skip list的probability参数。
        /// </summary>
        public double ProbabilityOfSkipList
        {
            get { return probabilityOfSkipList; }
            set
            {
                if (probabilityOfSkipList != value)
                {
                    probabilityOfSkipList = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// <see cref="BlockCache.sunkBlocksInMomery"/>能存储的<see cref="Block"/>数目的最大值。
        /// </summary>
        public long MaxSunkCountInMemory
        {
            get { return maxSunkCountInMemory; }
            set
            {
                if (maxSunkCountInMemory != value)
                {
                    maxSunkCountInMemory = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 等带解锁的时间长度。
        /// </summary>
        public TimeSpan LockTimeout
        {
            get { return lockTimeout; }
            set
            {
                if (lockTimeout != value)
                {
                    lockTimeout = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 安排所有文件指针。如果全部安排完毕，返回true，否则返回false。
        /// </summary>
        /// <returns></returns>
        public override bool ArrangePos()
        {
            return true;// 此类型比较特殊，应该在更新时立即指定各项文件指针。
        }

        /// <summary>
        /// 数据库文件的头部。应该放在数据库文件的最开始。
        /// </summary>
        public DBHeaderBlock() { }

#if DEBUG
        const string strBlockCount = "A";
#endif
        const string strFirstTablePagePos = "B";
        const string strFirstIndexPagePos = "C";
        const string strFirstSkipListNodePagePos = "D";
        const string strFirstDataPagePos = "E";
        const string strFirstEmptyPagePos = "F";
        const string strMaxLevel = "G";
        const string strProbability = "H";
        const string strMaxSunkCountInMemory = "I";
        const string strLockTimeout = "J";

        /// <summary>
        /// 序列化时系统会调用此方法。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

#if DEBUG
            info.AddValue(strBlockCount, this.blockCount);
#endif
            info.AddValue(strFirstTablePagePos, this.FirstTablePagePos);
            info.AddValue(strFirstIndexPagePos, this.FirstIndexPagePos);
            info.AddValue(strFirstSkipListNodePagePos, this.FirstSkipListNodePagePos);
            info.AddValue(strFirstDataPagePos, this.FirstDataPagePos);
            info.AddValue(strFirstEmptyPagePos, this.FirstEmptyPagePos);
            info.AddValue(strMaxLevel, this.MaxLevelOfSkipList);
            info.AddValue(strProbability, this.ProbabilityOfSkipList);
            info.AddValue(strMaxSunkCountInMemory, this.MaxSunkCountInMemory);
            info.AddValue(strLockTimeout, this.LockTimeout);
        }

        /// <summary>
        /// BinaryFormatter会通过调用此方法来反序列化此块。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DBHeaderBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            this.blockCount = info.GetInt64(strBlockCount);
#endif
            this.FirstTablePagePos = info.GetInt64(strFirstTablePagePos);
            this.FirstIndexPagePos = info.GetInt64(strFirstIndexPagePos);
            this.FirstSkipListNodePagePos = info.GetInt64(strFirstSkipListNodePagePos);
            this.FirstDataPagePos = info.GetInt64(strFirstDataPagePos);
            this.FirstEmptyPagePos = info.GetInt64(strFirstEmptyPagePos);
            this.MaxLevelOfSkipList = info.GetInt32(strMaxLevel);
            this.ProbabilityOfSkipList = info.GetDouble(strProbability);
            this.MaxSunkCountInMemory = info.GetInt64(strMaxSunkCountInMemory);
            this.LockTimeout = (TimeSpan)info.GetValue(strLockTimeout, typeof(TimeSpan));

            //this.IsDirty = false;
        }


        //#region IUpdatable 成员

        /// <summary>
        /// 标识此块是否需要重新写入数据库文件。
        /// </summary>
        public bool IsDirty { get; set; }

        //#endregion

        /// <summary>
        /// 显示此块的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}, FirstTablePagePos: {1}, FirstIndexPagePos: {2}, FirstSkipListNodePagePos: {3}, FirstDataPagePos: {4}, FirstEmptyPagePos: {5}, MaxLevelOfSkipList: {6}, ProbabilityOfSkipList: {7}, MaxSunkCountInMemory: {8}, LockTimeout: {9}",
                base.ToString(),
                this.FirstTablePagePos,
                this.FirstIndexPagePos,
                this.FirstSkipListNodePagePos,
                this.FirstDataPagePos,
                this.FirstEmptyPagePos,
                this.MaxLevelOfSkipList,
                this.ProbabilityOfSkipList,
                this.MaxSunkCountInMemory,
                this.LockTimeout
                );
        }
    }
}
