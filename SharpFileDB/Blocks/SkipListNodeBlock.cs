using SharpFileDB.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 用于把skip list node存储到数据库文件的块。
    /// </summary>
    [Serializable]
    public class SkipListNodeBlock : Block, IFourSideLinked, IUpdatable
    {
        /// <summary>
        /// 此结点的Key所在位置。
        /// </summary>
        public long KeyPos { get; set; }

        /// <summary>
        /// 用于存储此结点的Key的块。
        /// </summary>
        public DataBlock Key { get;set; }

        /// <summary>
        /// 此结点的Value的第一个块所在位置。
        /// </summary>
        public long ValuePos { get; set; }

        /// <summary>
        /// 用于存储此结点的Value的块。
        /// </summary>
        public DataBlock[] Value { get; set; }


        /// <summary>
        /// 用于把skip list node存储到数据库文件的块。
        /// </summary>
        public SkipListNodeBlock() { }

        const string strKeyPos = "K";
        const string strValuePos = "V";
        const string strRightPos = "R";
        const string strDownPos = "D";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strKeyPos, this.KeyPos);
            info.AddValue(strValuePos, this.ValuePos);

            info.AddValue(strRightPos, this.RightPos);
            info.AddValue(strDownPos, this.DownPos);
        }

        protected SkipListNodeBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.KeyPos = info.GetInt64(strKeyPos);
            this.ValuePos = info.GetInt64(strValuePos);

            this.RightPos = info.GetInt64(strRightPos);
            this.DownPos = info.GetInt64(strDownPos);
        }

        #region IFourSideLinked 成员

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public long LeftPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public IFourSideLinked LeftObj { get; set; }

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long RightPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public IFourSideLinked RightObj { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public long UpPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public IFourSideLinked UpObj { get; set; }

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long DownPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public IFourSideLinked DownObj { get; set; }

        #endregion

        #region IUpdatable 成员

        public bool IsDirty { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, key: {1}, value: {2}, down: {3}, right: {4}",
                base.ToString(),
                this.KeyPos, this.ValuePos, this.DownPos, this.RightPos);
        }
    }
}
