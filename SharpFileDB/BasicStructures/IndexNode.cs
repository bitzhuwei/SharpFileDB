using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SharpFileDB.BasicStructures
{
    /// <summary>
    /// 数据库的一个Table的一个Index。
    /// <para>可能是从数据库文件中得到的，也可能是即将保存到数据库文件的。</para>
    /// <para>IndexNode保存了skip list的第一个结点在数据库文件中的位置。<see cref="IndexNode.FirstSkipListNode"/></para>
    /// </summary>
    [Serializable]
    public class IndexNode : IDoubleLinkedNode, ISerializable
    {

        /// <summary>
        /// Index的名称，对应table-type的字段/属性名。
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Skip list的参数之一：max level。
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Skip list的参数之一：probability。
        /// </summary>
        public double Probability { get; set; }

        /// <summary>
        /// 此索引代表的skip list的第一个结点。
        /// </summary>
        public long FirstSkipListNode { get; set; }

        #region IDoubleLinkedNode 成员

        long IDoubleLinkedNode.ThisPos { get; set; }

        long IDoubleLinkedNode.PreviousPos { get; set; }

        long IDoubleLinkedNode.NextPos { get; set; }

        IDoubleLinkedNode IDoubleLinkedNode.PreviousObj { get; set; }

        IDoubleLinkedNode IDoubleLinkedNode.NextObj { get; set; }

        #endregion

        public IndexNode()
        {
            IDoubleLinkedNode link = this;
            link.PreviousPos = long.MaxValue;
            link.ThisPos = long.MaxValue;
            link.NextPos = long.MaxValue;
        }

        const string strIndexName = "t";

        const string strMaxLevel = "l";
        const string strProbability = "p";
        const string strFirstSkipListNode = "f";

        const string strNext = "n";

        #region ISerializable 成员

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.AddValue(strTableType, this.TableType.FullName);
            info.AddValue(strIndexName, this.IndexName);
            info.AddValue(strFirstSkipListNode, this.FirstSkipListNode);
            info.AddValue(strMaxLevel, this.MaxLevel);
            info.AddValue(strProbability, this.Probability);

            IDoubleLinkedNode link = this;
            info.AddValue(strNext, link.NextPos);
        }

        #endregion

        protected IndexNode(SerializationInfo info, StreamingContext context)
        {
            //string fullname = info.GetString(strTableType);
            //this.TableType = Type.GetType(fullname);
            this.IndexName = info.GetString(strIndexName);
            this.FirstSkipListNode = info.GetInt64(strFirstSkipListNode);
            this.MaxLevel = info.GetInt32(strMaxLevel);
            this.Probability = info.GetDouble(strProbability);

            IDoubleLinkedNode link = this;
            link.NextPos = info.GetInt64(strNext);
        }

    }
}
