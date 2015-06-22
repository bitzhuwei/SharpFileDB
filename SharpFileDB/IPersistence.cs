using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 文件数据库使用此接口进行持久化相关的操作。
    /// <para>File database executes persistence operations via this interface.</para>
    /// </summary>
    public interface IPersistence
    {
        /// <summary>
        /// <see cref="FileObject"/>文件的扩展名。
        /// Extension name of <see cref="FileObject"/>'s file.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// 将文件对象序列化为字符串。
        /// <para>Serialize the specified <paramref name="item"/> to string.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        void Serialize(FileObject item, string fullname);

        /// <summary>
        /// 将字符串反序列化成文件对象。
        /// <para>Deserialize the specified string to an instance of <paramref name="TFileObject"/>.</para>
        /// </summary>
        /// <typeparam name="TFileObject"></typeparam>
        /// <param name="serializedFileObject"></param>
        /// <returns></returns>
        TFileObject Deserialize<TFileObject>(string fullname) where TFileObject : FileObject;
    }
}
