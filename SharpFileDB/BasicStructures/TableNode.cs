using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.BasicStructures
{
    /// <summary>
    /// 数据库的一个Table。
    /// <para>可能是从数据库文件中得到的，也可能是即将保存到数据库文件的。</para>
    /// <para>TableNode是一个在数据库文件中保存的单链表的结点。</para>
    /// </summary>
    [Serializable]
    public class TableNode : IDoubleLinkedNode, ISerializable
    {
        /// <summary>
        /// 用户自定义的数据库Table的类型。
        /// <para>例如用户定义：
        /// <code>class User
        /// {
        ///     string Name;
        ///     string Id;
        ///     string phone;
        ///     DateTime birthday;
        /// }</code>
        /// 那么这个TableType就是Typeof(User)</para>
        /// </summary>
        public Type TableType { get; set; }

        /// <summary>
        /// 此Table内的所有Index组成的链表的头结点在数据库文件中的位置。
        /// <para>头结点不保存任何实际数据，只保存链表的第一个有用结点的位置。</para>
        /// <para>此头结点内只有((IFourSideLinkedNode)IndexHeadNode).NextPos指向第一个有用的结点。</para>
        /// </summary>
        public long IndexHeadNodePos { get; set; }

        #region IDoubleLinked 成员

        long IDoubleLinkedNode.ThisPos { get; set; }

        long IDoubleLinkedNode.PreviousPos { get; set; }

        long IDoubleLinkedNode.NextPos { get; set; }

        IDoubleLinkedNode IDoubleLinkedNode.PreviousObj { get; set; }

        IDoubleLinkedNode IDoubleLinkedNode.NextObj { get; set; }

        #endregion


        public TableNode()
        {
            IDoubleLinkedNode link = this;
            link.PreviousPos = long.MaxValue;
            link.ThisPos = long.MaxValue;
            link.NextPos = long.MaxValue;
        }

        const string strTableType = "t";
        const string strIndexHeadNode = "h";
        const string strNext = "n";

        #region ISerializable 成员

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue(strTableType, this.TableType);
            info.AddValue(strTableType, this.TableType.FullName);
            info.AddValue(strIndexHeadNode, this.IndexHeadNodePos);

            IDoubleLinkedNode link = this;
            info.AddValue(strNext, link.NextPos);
        }

        #endregion

        protected TableNode(SerializationInfo info, StreamingContext context)
        {
            //this.TableType = (Type)info.GetValue(strTableType, typeof(Type));
            string fullname = info.GetString(strTableType);
            this.TableType = Type.GetType(fullname);
            this.IndexHeadNodePos = info.GetInt64(strIndexHeadNode);

            IDoubleLinkedNode link = this;
            link.NextPos = info.GetInt64(strNext);
        }

    }
}
