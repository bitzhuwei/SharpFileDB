using SharpFileDB.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace SharpFileDB.Blocks
{
    /// <summary>
    /// 页的头部，可代表一个页。此块应在每个页的最开始处。
    /// </summary>
    [Serializable]
    public class PageHeaderBlock : Block//, IUpdatable
    {

        private Int16 availableBytes;
        private Int16 occupiedBytes;
        private long nextPagePos;

        /// <summary>
        /// 此页剩余的可用字节数。剩余的可用字节都在页的末尾。
        /// <para>剩余可用字节数+被使用的字节数+页中间那些七零八落的空白字节数（由删除操作造成）=页长度（4KB）</para>
        /// </summary>
        public Int16 AvailableBytes
        {
            get { return availableBytes; }
            set
            {
                if (availableBytes != value)
                {
                    availableBytes = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 此页内已被使用的字节数。
        /// <para>剩余可用字节数+被使用的字节数+页中间那些七零八落的空白字节数（由删除操作造成）=页长度（4KB）</para>
        /// </summary>
        public Int16 OccupiedBytes
        {
            get { return occupiedBytes; }
            set
            {
                if (occupiedBytes != value)
                {
                    occupiedBytes = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 此页的下一页的位置。
        /// <para>只有当此页为空白页时，此值才有效。</para>
        /// </summary>
        public long NextPagePos
        {
            get { return nextPagePos; }
            set
            {
                if (nextPagePos != value)
                {
                    nextPagePos = value;
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// 标识此块是否需要重新写入数据库文件。
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// 安排所有文件指针。如果全部安排完毕，返回true，否则返回false。
        /// </summary>
        /// <returns></returns>
        public override bool ArrangePos()
        {
            return true;// 此类型比较特殊，应该在创建时就为其安排好NextPagePos等属性。
        }

        /// <summary>
        /// 页的头部，可代表一个页。此块应在每个页的最开始处。
        /// </summary>
        public PageHeaderBlock() { }

        const string strAvailableBytes = "A";
        const string strOccupiedBytes = "B";
        const string strNextPagePos = "C";

        /// <summary>
        /// 序列化时系统会调用此方法。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(strAvailableBytes, this.AvailableBytes);
            info.AddValue(strOccupiedBytes, this.OccupiedBytes);
            info.AddValue(strNextPagePos, this.NextPagePos);
        }

        /// <summary>
        /// BinaryFormatter会通过调用此方法来反序列化此块。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected PageHeaderBlock(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.AvailableBytes = info.GetInt16(strAvailableBytes);
            this.OccupiedBytes = info.GetInt16(strOccupiedBytes);
            this.NextPagePos = info.GetInt64(strNextPagePos);

            this.IsDirty = false;
        }

        /// <summary>
        /// 显示此块的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}, OccupiedBytes: {1}, AvailableBytes: {2}, NextPagePos: {3}{4}",
                base.ToString(),
                this.OccupiedBytes,
                this.AvailableBytes,
                this.NextPagePos,
                this.AvailableBytes == Consts.maxAvailableSpaceInPage ? " Empty Page" : ""
                );
        }
    }
}
