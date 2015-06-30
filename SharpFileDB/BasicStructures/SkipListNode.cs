using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.BasicStructures
{
    /// <summary>
    /// skip list结点。可用于记录一个索引结点。
    /// <para>可能是从数据库文件中得到的，也可能是即将保存到数据库文件的。</para>
    /// <para>SkipListNode是一个在数据库文件中保存的skip list的结点。在数据库文件中只保存NextPos和DownPos，不保存PreviousPos和UpPos。</para>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// </summary>
    [Serializable]
    class SkipListNode<TKey, TValue> : IFourSideLinked, ISerializable
    {
        public virtual TKey Key { get; set; }
        public virtual TValue Value { get; set; }

        public SkipListNode<TKey, TValue> Right { get; set; }
        public SkipListNode<TKey, TValue> Down { get; set; }

        public SkipListNode() { }
        public SkipListNode(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        #region IFourSideLinked 成员

        long IFourSideLinked.ThisPos { get; set; }

        long IFourSideLinked.LeftPos { get; set; }

        IFourSideLinked IFourSideLinked.LeftObj { get; set; }

        long IFourSideLinked.RightPos { get; set; }

        IFourSideLinked IFourSideLinked.RightObj
        {
            get { return this.Right; }
            set
            {
                this.Right = (SkipListNode<TKey, TValue>)value;
            }
        }

        long IFourSideLinked.UpPos { get; set; }

        IFourSideLinked IFourSideLinked.UpObj { get; set; }

        long IFourSideLinked.DownPos { get; set; }

        IFourSideLinked IFourSideLinked.DownObj
        {
            get { return this.Down; }
            set
            {
                this.Down = (SkipListNode<TKey, TValue>)value;
            }
        }

        #endregion

        const string strKey = "k";
        const string strValue = "v";
        const string strDown = "d";
        const string strRight = "r";

        #region ISerializable 成员

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strKey, this.Key);
            info.AddValue(strValue, this.Value);

            IFourSideLinked link = this;
            info.AddValue(strDown, link.DownPos);
            info.AddValue(strRight, link.RightPos);
        }

        #endregion

        protected SkipListNode(SerializationInfo info, StreamingContext context)
        {
            this.Key = (TKey)info.GetValue(strKey, typeof(TKey));
            this.Value = (TValue)info.GetValue(strValue, typeof(TValue));

            IFourSideLinked link = this;
            link.DownPos = info.GetInt64(strDown);
            link.RightPos = info.GetInt64(strRight);
        }
    }
}
