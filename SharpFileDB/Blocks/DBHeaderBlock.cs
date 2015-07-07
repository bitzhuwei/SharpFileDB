using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 数据库文件的头部。应该放在数据库文件的最开始。
    /// </summary>
    [Serializable]
    internal class DBHeaderBlock : Block//, IUpdatable
    {

        private long firstTablePagePos;
        private long firstIndexPagePos;
        private long firstSkipListNodePagePos;
        private long firstDataPagePos;
        private long firstEmptyPagePos;

        private int maxLevelOfSkipList;
        private double probabilityOfSkipList;

        /// <summary>
        /// 第一个存储<see cref="TableBlock"/>的页的位置。
        /// </summary>
        internal long FirstTablePagePos
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
        internal long FirstIndexPagePos
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
        internal long FirstSkipListNodePagePos
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
        internal long FirstDataPagePos
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
        internal long FirstEmptyPagePos
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
        internal int MaxLevelOfSkipList
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
        internal double ProbabilityOfSkipList
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

        internal override bool ArrangePos()
        {
            return true;// 此类型比较特殊，应该在更新时立即指定各项文件指针。
        }

        ///// <summary>
        ///// <see cref="TableBlock"/>的头结点。
        ///// <para>头结点的<see cref="TableBlock.TableType"/>属性始终为空，所以<see cref="DBHeaderBlock"/>的序列化长度是不变的。</para>
        ///// </summary>
        //internal TableBlock TableBlockHead { get; set; }

        /// <summary>
        /// 数据库文件的头部。应该放在数据库文件的最开始。
        /// </summary>
        internal DBHeaderBlock() { }

        const string strFirstTablePagePos = "T";
        const string strFirstIndexPagePos = "I";
        const string strFirstSkipListNodePagePos = "S";
        const string strFirstDataPagePos = "D";
        const string strFirstEmptyPagePos = "E";
        const string strMaxLevel = "M";
        const string strProbability = "P";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strFirstTablePagePos, this.FirstTablePagePos);
            info.AddValue(strFirstIndexPagePos, this.FirstIndexPagePos);
            info.AddValue(strFirstSkipListNodePagePos, this.FirstSkipListNodePagePos);
            info.AddValue(strFirstDataPagePos, this.FirstDataPagePos);
            info.AddValue(strFirstEmptyPagePos, this.FirstEmptyPagePos);
            info.AddValue(strMaxLevel, this.MaxLevelOfSkipList);
            info.AddValue(strProbability, this.ProbabilityOfSkipList);
        }

        protected DBHeaderBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.FirstTablePagePos = info.GetInt64(strFirstTablePagePos);
            this.FirstIndexPagePos = info.GetInt64(strFirstIndexPagePos);
            this.FirstSkipListNodePagePos = info.GetInt64(strFirstSkipListNodePagePos);
            this.FirstDataPagePos = info.GetInt64(strFirstDataPagePos);
            this.FirstEmptyPagePos = info.GetInt64(strFirstEmptyPagePos);
            this.MaxLevelOfSkipList = info.GetInt32(strMaxLevel);
            this.ProbabilityOfSkipList = info.GetDouble(strProbability);
        }


        #region IUpdatable 成员

        public bool IsDirty { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, FirstTablePagePos: {1}, FirstIndexPagePos: {2}, FirstSkipListNodePagePos: {3}, FirstDataPagePos: {4}, FirstEmptyPagePos: {5}, MaxLevelOfSkipList: {6}, ProbabilityOfSkipList: {7}",
                base.ToString(),
                this.FirstTablePagePos,
                this.FirstIndexPagePos,
                this.FirstSkipListNodePagePos,
                this.FirstDataPagePos,
                this.FirstEmptyPagePos,
                this.MaxLevelOfSkipList,
                this.ProbabilityOfSkipList
                );
        }
    }
}
