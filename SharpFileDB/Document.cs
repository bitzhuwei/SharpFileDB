using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SharpFileDB
{
    /// <summary>
    /// 可在文件数据库中使用CRUD操作的所有类型的基类。类似于关系数据库中的Table。
    /// Base class for all classed that can use CRUD in SharpFileDB. It's similar to the concept 'table' in relational database.
    /// </summary>
    [Serializable]
    public abstract class Document : ISerializable
    {

        /// <summary>
        /// 用以区分每个Table的每条记录。
        /// This Id is used for diffrentiate instances of 'table's.
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        /// 创建一个文件对象，在用<code>FileDBContext.Create();</code>将此对象保存到数据库之前，此对象的Id为<code>Guid.Empty</code>。
        /// <para>Create a <see cref="Document"/> whose Id is <code>Guid.Empty</code> until it's saved to database by <code>FileDBContext.Create();</code>.</para>
        /// </summary>
        public Document()
        {
        }

        public override string ToString()
        {
            return string.Format("Id: {0}", this.Id);
        }

        /// <summary>
        /// 使用的字符越少，序列化时占用的字节就越少。一个字符都不用最好。
        /// <para>Using less chars means less bytes after serialization. And "" is allowed.</para>
        /// </summary>
        const string strGuid = "";

        #region ISerializable 成员

        /// <summary>
        /// This method will be invoked automatically when IFormatter.Serialize() is called.
        /// <para>You must use <code>base(info, context);</code> in the derived class to feed <see cref="Document"/>'s fields and properties.</para>
        /// <para>当使用IFormatter.Serialize()时会自动调用此方法。</para>
        /// <para>继承此类型时，必须在子类型中用<code>base(info, context);</code>来填充<see cref="Document"/>自身的数据。</para>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string id = this.Id.ToString();
            info.AddValue(strGuid, id);
        }

        #endregion

        /// <summary>
        /// This method will be invoked automatically when IFormatter.Serialize() is called.
        /// <para>You must use <code>: base(info, context)</code> in the derived class to feed <see cref="Document"/>'s fields and properties.</para>
        /// <para>当使用IFormatter.Serialize()时会自动调用此方法。</para>
        /// <para>继承此类型时，必须在子类型中用<code>: base(info, context)</code>来填充<see cref="Document"/>自身的数据。</para>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Document(SerializationInfo info, StreamingContext context)
        {
            string str = info.GetString(strGuid);
            this.Id = Guid.Parse(str);
        }

    }
}
