using SharpFileDB.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 用于把skip list node存储到数据库文件的块。
    /// </summary>
    [Serializable]
    public class SkipListNodeBlock : AllocBlock, ISkipListNode<SkipListNodeBlock>//, IUpdatable
    {
        /// <summary>
        /// 此结点的Key所在位置。
        /// </summary>
        public long KeyPos { get; set; }

        /// <summary>
        /// 用于存储此结点的Key的块。
        /// </summary>
        public DataBlock Key { get; set; }

        /// <summary>
        /// 此结点的Value的第一个块所在位置。
        /// </summary>
        public long ValuePos { get; set; }

        /// <summary>
        /// 用于存储此结点的Value的块。
        /// </summary>
        public DataBlock[] Value { get; set; }

        /// <summary>
        /// 安排所有文件指针。如果全部安排完毕，返回true，否则返回false。
        /// </summary>
        /// <returns></returns>
        public override bool ArrangePos()
        {
            bool allArranged = true;

            if (this.Key != null)
            {
                if (this.Key.ThisPos != 0)
                { this.KeyPos = this.Key.ThisPos; }
                else
                { allArranged = false; }
            }

            if (this.Value != null)
            {
                if (this.Value[0].ThisPos != 0)
                { this.ValuePos = this.Value[0].ThisPos; }
                else
                { allArranged = false; }
            }

            if (this.DownObj != null)// 此结点不是最下方的结点。
            {
                if (this.DownObj.ThisPos != 0)
                { this.DownPos = this.DownObj.ThisPos; }
                else
                { allArranged = false; }
            }

            if (this.RightObj != null)// 此结点不是最右方的结点。
            {
                if (this.RightObj.ThisPos != 0)
                { this.RightPos = this.RightObj.ThisPos; }
                else
                { allArranged = false; }
            }

            return allArranged;
        }

        /// <summary>
        /// 用于把skip list node存储到数据库文件的块。
        /// 此时代表的Key和Value都为null，代表一个头结点。
        /// </summary>
        public SkipListNodeBlock() { }

        const string strKeyPos = "A";
        const string strValuePos = "B";
        const string strRightPos = "C";
        const string strDownPos = "D";

        /// <summary>
        /// 序列化时系统会调用此方法。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(strKeyPos, this.KeyPos);
            info.AddValue(strValuePos, this.ValuePos);

            info.AddValue(strRightPos, this.RightPos);
            info.AddValue(strDownPos, this.DownPos);
        }

        /// <summary>
        /// BinaryFormatter会通过调用此方法来反序列化此块。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SkipListNodeBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.KeyPos = info.GetInt64(strKeyPos);
            this.ValuePos = info.GetInt64(strValuePos);

            this.RightPos = info.GetInt64(strRightPos);
            this.DownPos = info.GetInt64(strDownPos);
        }

        #region IFourSideLinked 成员

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long RightPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public SkipListNodeBlock RightObj { get; set; }

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long DownPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public SkipListNodeBlock DownObj { get; set; }

        #endregion

        /// <summary>
        /// 显示此块的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}, KeyPos: {1}, ValuePos: {2}, DownPos: {3}, RightPos: {4}",
                base.ToString(),
                this.KeyPos, this.ValuePos, this.DownPos, this.RightPos);
        }
    }
}
