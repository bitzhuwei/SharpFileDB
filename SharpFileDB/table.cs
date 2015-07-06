using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SharpFileDB
{
    /// <summary>
    /// 继承此类型以实现您需要的Table。
    /// </summary>
    [Serializable]
    public abstract class Table : ISerializable
    {

        /// <summary>
        /// 用以区分每个Table的每条记录。
        /// This Id is used for diffrentiate instances of 'table's.
        /// </summary>
        [TableIndex]// 标记为索引，这是每个表都有的主键。
        public ObjectId Id { get; set; }

        /// <summary>
        /// 创建一个文件对象，在用<code>FileDBContext.Insert();</code>将此对象保存到数据库之前，此对象的Id为null。
        /// </summary>
        public Table()
        {
        }

        /// <summary>
        /// 显示此条记录的Id。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Id: {0}", this.Id);
        }

        /// <summary>
        /// 使用的字符越少，序列化时占用的字节就越少。一个字符都不用最好。
        /// <para>Using less chars means less bytes after serialization. And "" is allowed.</para>
        /// </summary>
        const string strId = "";

        #region ISerializable 成员

        /// <summary>
        /// This method will be invoked automatically when IFormatter.Serialize() is called.
        /// <para>You must use <code>base(info, context);</code> in the derived class to feed <see cref="Table"/>'s fields and properties.</para>
        /// <para>当使用IFormatter.Serialize()时会自动调用此方法。</para>
        /// <para>继承此类型时，必须在子类型中用<code>base(info, context);</code>来填充<see cref="Table"/>自身的数据。</para>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // 187
            //string id = this.Id.ToString();//这比用byte[]占的字节多
            // 185
            byte[] value = this.Id.Value;

            info.AddValue(strId, value);
        }

        #endregion

        /// <summary>
        /// This method will be invoked automatically when IFormatter.Serialize() is called.
        /// <para>You must use <code>: base(info, context)</code> in the derived class to feed <see cref="Table"/>'s fields and properties.</para>
        /// <para>当使用IFormatter.Serialize()时会自动调用此方法。</para>
        /// <para>继承此类型时，必须在子类型中用<code>: base(info, context)</code>来填充<see cref="Table"/>自身的数据。</para>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Table(SerializationInfo info, StreamingContext context)
        {
            //string str = info.GetString(strId);
            //this.Id = new DocumentId(str);
            byte[] value = (byte[])info.GetValue(strId, typeof(byte[]));
            this.Id = new ObjectId(value);
        }

    }
}
