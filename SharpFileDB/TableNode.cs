using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 用于存储到数据库文件中的一个表的结点。
    /// <para>Node of a table of database.</para>
    /// <para>Table node</para>
    /// </summary>
    [Serializable]
    internal class TableNode : ISerializable
    {
        /// <summary>
        /// 此结点在SkipList表中的下一个结点存储在数据库文件中的起始位置。
        /// <para>The start position of next node in the skip list in database file.</para>
        /// </summary>
        public long NextNodePosition { get; set; }

        /// <summary>
        /// 此结点包含的<see cref="DocumentNode"/>对象保存到数据库文件中的位置。
        /// <para>Th start position of <see cref="DocumentNode"/> oject stored in database file.</para>
        /// </summary>
        public long DocumentNodePosition { get; set; }

        public override string ToString()
        {
            return string.Format("doc node: {0}, next table node: {1}", DocumentNodePosition, NextNodePosition);
        }

        public TableNode() { }

        const string strDocumentNodePosition = "c";
        const string strNextPosition = "n";

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strDocumentNodePosition, this.DocumentNodePosition);
            info.AddValue(strNextPosition, this.NextNodePosition);
        }

        #endregion

        protected TableNode(SerializationInfo info, StreamingContext context)
        {
            this.DocumentNodePosition = info.GetInt64(strDocumentNodePosition);
            this.NextNodePosition = info.GetInt64(strNextPosition);
        }
    }
}
