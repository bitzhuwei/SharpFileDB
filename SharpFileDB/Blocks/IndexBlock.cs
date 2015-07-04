using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 存储索引的块。此块在内存中充当skip list。
    /// </summary>
    [Serializable]
    public class IndexBlock : Block, IDoubleLinkedNode
    {

        /// <summary>
        /// 此Index的第一个Skip List结点（this.SkipListHeadNodes.Last()）的位置。
        /// </summary>
        public long SkipListHeadNodePos { get; set; }

        /// <summary>
        /// 此索引的第一列skip list结点。是skip list的头结点。
        /// 用于skip list的增删。
        /// </summary>
        public SkipListNodeBlock[] SkipListHeadNodes { get; set; }

        /// <summary>
        /// 用于加速skip list的增删。
        /// </summary>
        internal int CurrentLevel { get; set; }

        /// <summary>
        /// 此Index代表的表的成员（字段/属性）名。
        /// </summary>
        public string BindMember { get; set; }

        /// <summary>
        /// 存储索引的块。
        /// </summary>
        public IndexBlock() { }

        const string strSkipListNodePos = "S";
        const string strBindMember = "M";

        const string strNext = "N";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue(strSkipListNodePos, this.SkipListHeadNodePos);
            info.AddValue(strBindMember, this.BindMember);

            info.AddValue(strNext, this.NextPos);
        }

        protected IndexBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.SkipListHeadNodePos = info.GetInt64(strSkipListNodePos);
            this.BindMember = info.GetString(strBindMember);

            this.NextPos = info.GetInt64(strNext);
        }


        #region IDoubleLinkedNode 成员

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public long PreviousPos { get; set; }

        /// <summary>
        /// 数据库中保存此值。
        /// </summary>
        public long NextPos { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public IDoubleLinkedNode PreviousObj { get; set; }

        /// <summary>
        /// 数据库中不保存此值。
        /// </summary>
        public IDoubleLinkedNode NextObj { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, S: {1}, M: {2}, next: {3}, pre: {4}",
                base.ToString(),
                this.SkipListHeadNodePos,
                this.BindMember,
                this.NextPos,
                this.PreviousPos);
        }
    }
}
