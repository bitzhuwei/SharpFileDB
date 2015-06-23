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
    public abstract class FileObject : ISerializable
    {
        /// <summary>
        /// 用以区分每个Table的每条记录。
        /// This Id is used for diffrentiate instances of 'table's.
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        /// 创建一个文件对象，在用<code>FileDBContext.Create();</code>将此对象保存到数据库之前，此对象的Id为<code>Guid.Empty</code>。
        /// <para>Create a <see cref="FileObject"/> whose Id is <code>Guid.Empty</code> until it's saved to database by <code>FileDBContext.Create();</code>.</para>
        /// </summary>
        public FileObject()
        {
        }

        /// <summary>
        /// 生成文件名，此文件将用于存储此<see cref="FileObject"/>的内容。
        /// Generate file name that will contain this instance's data of <see cref="FileObject"/>.
        /// </summary>
        /// <param name="extension">文件扩展名。<para>File's extension name.</para></param>
        /// <returns></returns>
        internal string GenerateFileName(string extension)
        {
            string id = this.Id.ToString();

            string name = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", id, extension);

            return name;
        }

        public override string ToString()
        {
            return string.Format("Id: {0}", this.Id);
        }

        const string strGuid = "Guid";

        #region ISerializable 成员

        /// <summary>
        /// This method will be invoked automatically when IFormatter.Serialize() is called.
        /// <para>You must use <code>base(info, context);</code> in the derived class to feed <see cref="FileObject"/>'s fields and properties.</para>
        /// <para>当使用IFormatter.Serialize()时会自动调用此方法。</para>
        /// <para>继承此类型时，必须在子类型中用<code>base(info, context);</code>来填充<see cref="FileObject"/>自身的数据。</para>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strGuid, this.Id.ToString());
        }

        #endregion

        /// <summary>
        /// This method will be invoked automatically when IFormatter.Serialize() is called.
        /// <para>You must use <code>: base(info, context)</code> in the derived class to feed <see cref="FileObject"/>'s fields and properties.</para>
        /// <para>当使用IFormatter.Serialize()时会自动调用此方法。</para>
        /// <para>继承此类型时，必须在子类型中用<code>: base(info, context)</code>来填充<see cref="FileObject"/>自身的数据。</para>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FileObject(SerializationInfo info, StreamingContext context)
        {
            string str = (string)info.GetValue(strGuid, typeof(string));
            this.Id = Guid.Parse(str);
        }
    }
}
