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
    internal class TableNode : ISerializable, IPointToNextInFile
    {

        /// <summary>
        /// 此结点包含的<see cref="DocumentNode"/>对象保存到数据库文件中的位置。
        /// <para>Th start position of <see cref="DocumentNode"/> oject stored in database file.</para>
        /// </summary>
        public long DocumentNodePosition { get; set; }

        public override string ToString()
        {
            return string.Format("TableNode: doc node pos: {0}, next pos: {1}", 
                this.DocumentNodePosition, this.NextSerializedPositionInFile);
        }

        public TableNode() { }

        const string strDocumentNodePosition = "c";
        const string strNextSerializedPositionInFile = "n";

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strDocumentNodePosition, this.DocumentNodePosition);
            info.AddValue(strNextSerializedPositionInFile, this.NextSerializedPositionInFile);
        }

        #endregion

        protected TableNode(SerializationInfo info, StreamingContext context)
        {
            this.DocumentNodePosition = info.GetInt64(strDocumentNodePosition);
            this.NextSerializedPositionInFile = info.GetInt64(strNextSerializedPositionInFile);
        }



        #region IPointToNextInFile 成员

        public long SerializedPositionInFile { get; set; }

        public long SerializedLengthInFile { get; set; }

        public long NextSerializedPositionInFile { get; set; }

        #endregion
    }
}
