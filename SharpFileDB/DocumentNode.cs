using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 用于存储到数据库文件中的一条数据库记录的结点。
    /// <para>An item that store to database.</para>
    /// </summary>
    [Serializable]
    class DocumentNode : ISerializable, IPointToNextInFile
    {

        /// <summary>
        /// 此结点包含的<see cref="DocumentPosition"/>对象保存到数据库文件中的位置。
        /// <para>Th start position of <see cref="DocumentPosition"/> oject stored in database file.</para>
        /// </summary>
        public long DocumentPosition { get; set; }

        public override string ToString()
        {
            return string.Format("DocumentNode: doc pos: {0}, next pos: {1}", 
                this.DocumentPosition, this.NextSerializedPositionInFile);
        }

        public DocumentNode() { }

        const string strDocumentPosition = "c";
        const string strNextSerializedPositionInFile = "n";

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strDocumentPosition, this.DocumentPosition);
            info.AddValue(strNextSerializedPositionInFile, this.NextSerializedPositionInFile);
        }

        #endregion

        protected DocumentNode(SerializationInfo info, StreamingContext context)
        {
            this.DocumentPosition = info.GetInt64(strDocumentPosition);
            this.NextSerializedPositionInFile = info.GetInt64(strNextSerializedPositionInFile);
        }


        #region IPointToNextInFile 成员

        public long SerializedPositionInFile { get; set; }

        public long SerializedLengthInFile { get; set; }

        public long NextSerializedPositionInFile { get; set; }

        #endregion
    }
}
