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
    internal class SkipListNodeBlock : AllocBlock, ISkipListNode<SkipListNodeBlock>//, IUpdatable
    {
        /// <summary>
        /// 此结点的Key所在位置。
        /// </summary>
        internal long KeyPos { get; set; }

        /// <summary>
        /// 用于存储此结点的Key的块。
        /// </summary>
        internal DataBlock Key { get;set; }

        /// <summary>
        /// 此结点的Value的第一个块所在位置。
        /// </summary>
        internal long ValuePos { get; set; }

        /// <summary>
        /// 用于存储此结点的Value的块。
        /// </summary>
        internal DataBlock[] Value { get; set; }

        internal override bool ArrangePos()
        {
            bool allArranged = true;

            if (this.Key != null)
            {
                //if (this.KeyPos == 0)// 尚未被赋值。
                {
                    if (this.Key.ThisPos != 0)
                    { this.KeyPos = this.Key.ThisPos; }
                    else
                    { allArranged = false; }
                }
            }

            if (this.Value != null)
            {
                //if (this.ValuePos == 0)// 尚未被赋值。
                {
                    if (this.Value[0].ThisPos != 0)
                    { this.ValuePos = this.Value[0].ThisPos; }
                    else
                    { allArranged = false; }
                }
            }

            if (this.DownObj != null)// 此结点不是最下方的结点。
            {
                //if (this.DownPos == 0)// 尚未被赋值。
                {
                    if (this.DownObj.ThisPos != 0)
                    { this.DownPos = this.DownObj.ThisPos; }
                    else
                    { allArranged = false; }
                }
            }

            if (this.RightObj != null)// 此结点不是最右方的结点。
            {
                //if (this.RightPos == 0)// this.RightPos可能是旧值，所以此处不应以0作为判定标志。//尚未被赋值。
                {
                    if (this.RightObj.ThisPos != 0)
                    { this.RightPos = this.RightObj.ThisPos; }
                    else
                    { allArranged = false; }
                }
            }

            return allArranged;
        }

        ///// <summary>
        ///// 获取Key和Value处于头结点状态的<see cref="SkipListNodeBlock"/>对象。
        ///// </summary>
        ///// <returns></returns>
        //internal static SkipListNodeBlock GetHeadNode()
        //{
        //    SkipListNodeBlock headNode = new SkipListNodeBlock();
        //    // 初始化时是头结点的状态。
        //    headNode.Key = new DataBlock() { ThisPos = -1, };
        //    headNode.Value = new DataBlock[0];

        //    return headNode;
        //}

        /// <summary>
        /// 用于把skip list node存储到数据库文件的块。
        /// 此时代表的Key和Value都为null，代表一个头结点。
        /// </summary>
        internal SkipListNodeBlock() { }

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
            : base(info, context)
        {
            this.KeyPos = info.GetInt64(strKeyPos);
            this.ValuePos = info.GetInt64(strValuePos);

            this.RightPos = info.GetInt64(strRightPos);
            this.DownPos = info.GetInt64(strDownPos);
        }

        #region IFourSideLinked 成员

        ///// <summary>
        ///// 数据库中不保存此值。
        ///// </summary>
        //public long LeftPos { get; set; }

        ///// <summary>
        ///// 数据库中不保存此值。
        ///// </summary>
        //public SkipListNodeBlock LeftObj { get; set; }

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long RightPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public SkipListNodeBlock RightObj { get; set; }

        ///// <summary>
        ///// 数据库中不保存此值。
        ///// </summary>
        //public long UpPos { get; set; }

        ///// <summary>
        ///// 数据库中不保存此值。
        ///// </summary>
        //public SkipListNodeBlock UpObj { get; set; }

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long DownPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public SkipListNodeBlock DownObj { get; set; }

        #endregion

        //#region IUpdatable 成员

        //public bool IsDirty { get; set; }

        //#endregion

        public override string ToString()
        {
            return string.Format("{0}, KeyPos: {1}, ValuePos: {2}, DownPos: {3}, RightPos: {4}",
                base.ToString(),
                this.KeyPos, this.ValuePos, this.DownPos, this.RightPos);
        }
    }
}
