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
    public class DBHeaderBlock : Block, IUpdatable
    {

        /// <summary>
        /// 第一个存储<see cref="TableBlock"/>的页的位置。
        /// </summary>
        public long FirstTablePagePos { get; set; }

        /// <summary>
        /// 第一个存储<see cref="IndexBlock"/>的页的位置。
        /// </summary>
        public long FirstIndexPagePos { get; set; }

        /// <summary>
        /// 第一个存储<see cref="SkipListNodeBlock"/>的页的位置。
        /// </summary>
        public long FirstSkipListNodePagePos { get; set; }

        /// <summary>
        /// 第一个存储<see cref="DataBlock"/>的页的位置。
        /// </summary>
        public long FirstDataPagePos { get; set; }

        /// <summary>
        /// 第一个存储空白页的位置。
        /// <para>当数据库删除某些内容后，可能会出现一些页不再被占用，此时它们就成为空白页。</para>
        /// </summary>
        public long FirstEmptyPagePos { get; set; }

        /// <summary>
        /// 索引使用的skip list的max level参数。
        /// </summary>
        public int MaxLevelOfSkipList { get; set; }
        
        /// <summary>
        /// 索引使用的skip list的probability参数。
        /// </summary>
        public double ProbabilityOfSkipList { get; set; }

        internal override bool ArrangePos()
        {
            return true;// 此类型比较特殊，应该在更新时立即指定各项文件指针。
        }

        ///// <summary>
        ///// <see cref="TableBlock"/>的头结点。
        ///// <para>头结点的<see cref="TableBlock.TableType"/>属性始终为空，所以<see cref="DBHeaderBlock"/>的序列化长度是不变的。</para>
        ///// </summary>
        //public TableBlock TableBlockHead { get; set; }

        /// <summary>
        /// 数据库文件的头部。应该放在数据库文件的最开始。
        /// </summary>
        public DBHeaderBlock()
        {
            //this.TableBlockHead = new TableBlock();
        }

        const string strFirstTablePagePos = "T";
        const string strFirstIndexPagePos = "I";
        const string strFirstSkipListNodePagePos = "S";
        const string strFirstDataPagePos = "D";
        const string strFirstEmptyPagePos = "E";
        const string strMaxLevel = "M";
        const string strProbability = "P";

        //const string strTableBlockHead = "H";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strFirstTablePagePos, this.FirstTablePagePos);
            info.AddValue(strFirstIndexPagePos, this.FirstIndexPagePos);
            info.AddValue(strFirstSkipListNodePagePos, this.FirstSkipListNodePagePos);
            info.AddValue(strFirstDataPagePos, this.FirstDataPagePos);
            info.AddValue(strFirstEmptyPagePos, this.FirstEmptyPagePos);
            info.AddValue(strMaxLevel, this.MaxLevelOfSkipList);
            info.AddValue(strProbability, this.ProbabilityOfSkipList);

            //info.AddValue(strTableBlockHead, this.TableBlockHead);
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

            //this.TableBlockHead = (TableBlock)info.GetValue(strTableBlockHead, typeof(TableBlock));
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
