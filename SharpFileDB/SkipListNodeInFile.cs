using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 在数据库文件中保存的skip list的结点。
    /// <para>Node of skip list that stores in database file.</para>
    /// </summary>
    [Serializable]
    class SkipListNodeInFile<TKey, TValue> : ISerializable, IPointToNextInFile
    {
        IFormatter formatter = new BinaryFormatter();

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipListNode&lt;TKey, TValue&gt;"/> class.
		/// </summary>
		internal SkipListNodeInFile()	{}

		/// <summary>
		/// Initializes a new instance of the <see cref="SkipListNode&lt;TKey, TValue&gt;"/> class.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="val">The value.</param>
        internal SkipListNodeInFile(TKey key, TValue val)
		{
			this.Key = key;
			this.Value = val;
		}


		#endregion

		#region Properties

        /// <summary>
        /// Key of skip list node in file.
        /// </summary>
        internal virtual TKey Key { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
        internal virtual TValue Value { get; set; }

		/// <summary>
		/// Gets or sets the right node.
		/// </summary>
		/// <value>The right node.</value>
        internal SkipListNodeInFile<TKey, TValue> Right
		{
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
		}

        public SkipListInFile<TKey, TValue> SkipList { get; set; }

		/// <summary>
		/// Gets or sets the down node.
		/// </summary>
		/// <value>The down node.</value>
        internal SkipListNodeInFile<TKey, TValue> Down
		{
            get
            {
                SkipListNodeInFile<TKey, TValue> result = null;
                long startPosition = this.NextSerializedPositionInFile;
                if (startPosition != 0)
                {
                    Stream stream = this.SkipList.Stream;
                    stream.Seek(startPosition, SeekOrigin.Begin);
                    object obj = formatter.Deserialize(stream);// result.NextPositionInFile should be deserialized in formatter.Deserialize(stream);.
                    long currentPosition = stream.Position;
                    result = (SkipListNodeInFile<TKey, TValue>)obj;
                    result.SerializedPositionInFile = startPosition;
                    result.SerializedLengthInFile = currentPosition - startPosition;
                }
                return result;
            }
            set { throw new NotImplementedException(); }
		}

		#endregion

        // This is Value property in DocumentNode.
        ///// <summary>
        ///// 此结点包含的<see cref="DocumentPosition"/>对象保存到数据库文件中的位置。
        ///// <para>Th start position of <see cref="DocumentPosition"/> oject stored in database file.</para>
        ///// </summary>
        //public long DocumentPosition { get; set; }



        /// <summary>
        /// Right pointer of skip list node in file.
        /// </summary>
        public long RightSerializedPositionInFile { get; set; }

        public override string ToString()
        {
            return string.Format("DocumentNode: value: {0}, next pos: {1}", 
                this.Value, this.NextSerializedPositionInFile);
                //this.DocumentPosition, this.NextSerializedPositionInFile);
        }

        protected const string strKey = "k";//key
        protected const string strValue = "v";//value
        protected const string strNextSerializedPositionInFile = "n";//next
        protected const string strRightSerializedPositionInFile = "r";//right

        #region ISerializable 成员

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strKey, this.Key);
            info.AddValue(strValue, this.Value, typeof(TValue));
            //info.AddValue(strDocumentPosition, this.DocumentPosition);
            info.AddValue(strNextSerializedPositionInFile, this.NextSerializedPositionInFile);
            info.AddValue(strRightSerializedPositionInFile, this.RightSerializedPositionInFile);
        }

        #endregion

        protected SkipListNodeInFile(SerializationInfo info, StreamingContext context)
        {
            this.Key = (TKey)info.GetValue(strKey, typeof(TKey));
            this.Value = (TValue)info.GetValue(strValue, typeof(TValue));
            //this.DocumentPosition = info.GetInt64(strValue);
            this.NextSerializedPositionInFile = info.GetInt64(strNextSerializedPositionInFile);
            this.RightSerializedPositionInFile = info.GetInt64(strRightSerializedPositionInFile);
        }


        #region IPointToNextInFile 成员

        public long SerializedPositionInFile { get; set; }

        public long SerializedLengthInFile { get; set; }

        public long NextSerializedPositionInFile { get; set; }

        #endregion
    }
}
