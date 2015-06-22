using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PostSharp.Patterns.Contracts;

namespace SharpFileDB
{
    /// <summary>
    /// 文件数据库。
    /// Represents a file database.
    /// </summary>
    public class FileDBContext
    {
        /// <summary>
        /// 文件数据库。
        /// <para>Represents a file database.</para>
        /// </summary>
        /// <param name="directory">数据库文件所在目录。<para>Directory for all files of database.</para></param>
        /// <param name="persistence">持久化方式。<para>The <see cref="IPersistence"/> instance.</para></param>
        public FileDBContext(string directory = null, IPersistence persistence = null)
        {
            if (directory == null)
            {
                this.Directory = Environment.CurrentDirectory;
            }
            else
            {
                Directory = directory;
            }

            if (persistence == null)
            {
                this.persistence = new DefaultPersistence(DefaultPersistence.PersistenceFormat.Soap);
            }
            else
            {
                this.persistence = persistence;
            }
        }

        public override string ToString()
        {
            return string.Format("@: {0}, IPersistence: {1}", Directory, persistence);
        }

        #region Properties

        /// <summary>
        /// 数据库文件所在目录
        /// <para>Directory of database files.</para>
        /// </summary>
        public virtual string Directory { get; protected set; }

        /// <summary>
        /// 文件数据库使用此接口进行持久化相关的操作。
        /// <para>FIle database executes persistence operations via this interface.</para>
        /// </summary>
        public virtual IPersistence persistence { get; set; }

        #endregion


        protected string GenerateFileFullPath([Required] FileObject item)
        {
            Type fileObjectType = item.GetType();
            string path = GenerateFilePath(fileObjectType);

            string name = item.GenerateFileName(this.persistence.Extension);

            string fullname = Path.Combine(path, name);

            return fullname;
        }

        /// <summary>
        /// 生成文件路径
        /// </summary>
        /// <typeparam name="TDocument">文档类型</typeparam>
        /// <returns>文件路径</returns>
        protected string GenerateFilePath([Required] Type type)
        {
            string path = Path.Combine(this.Directory, type.Name);

            return path;
        }

        #region CRUD

        /// <summary>
        /// 增加一个<see cref="FileObject"/>到数据库。这实际上创建了一个文件。
        /// <para>Create a new <see cref="FileObject"/> into database. This operation will create a new file.</para>
        /// </summary>
        /// <param name="item"></param>
        public virtual void Create([Required] FileObject item)
        {
            string fullname = GenerateFileFullPath(item);

            System.IO.FileInfo info = new System.IO.FileInfo(fullname);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);

            this.persistence.Serialize(item, fullname);
        }

        /// <summary>
        /// 检索符合给定条件的所有<paramref name="TFileObject"/>。
        /// <para>Retrives all <paramref name="TFileObject"/> that satisfies the specified condition.</para>
        /// </summary>
        /// <typeparam name="TFileObject"></typeparam>
        /// <param name="predicate">检索出的对象应满足的条件。<para>THe condition that should be satisfied by retrived object.</para></param>
        /// <returns></returns>
        public virtual IList<TFileObject> Retrieve<TFileObject>(Predicate<TFileObject> predicate)
            where TFileObject : FileObject
        {
            IList<TFileObject> result = new List<TFileObject>();

            if (predicate != null)
            {
                string path = GenerateFilePath(typeof(TFileObject));
                string[] files = System.IO.Directory.GetFiles(
                    path, "*." + this.persistence.Extension, SearchOption.AllDirectories);

                foreach (var fullname in files)
                {
                    TFileObject deserializedFileObject =
                        this.persistence.Deserialize<TFileObject>(fullname);

                    if (predicate(deserializedFileObject))
                    {
                        result.Add(deserializedFileObject);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 更新给定的对象。
        /// <para>Update specified <paramref name="FileObject"/>.</para>
        /// </summary>
        /// <param name="item">要被更新的对象。<para>The object to be updated.</para></param>
        public virtual void Update(FileObject item)
        {
            string fullname = GenerateFileFullPath(item);

            this.persistence.Serialize(item, fullname);
        }

        /// <summary>
        /// 删除指定的对象。
        /// <para>Delete specified <paramref name="FileObject"/>.</para>
        /// </summary>
        /// <param name="item">要被删除的对象。<para>The object to be deleted.</para></param>
        public virtual void Delete(FileObject item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(item.ToString());
            }

            string fullname = GenerateFileFullPath(item);

            if (File.Exists(fullname))
            {
                File.Delete(fullname);
            }
        }

        #endregion CRUD

    }
}