using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
                this.Directory = Path.Combine(Environment.CurrentDirectory, "SharpFileDB_BaseDir");
            }
            else
            {
                Directory = directory;
            }

            if (persistence == null)
            {
                this.persistence = new DefaultPersistence();
            }
            else
            {
                this.persistence = persistence;
            }

            System.IO.Directory.CreateDirectory(this.Directory);
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


        protected string GenerateFileFullPath(Document item)
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
        protected string GenerateFilePath(Type type)
        {
            string path = Path.Combine(this.Directory, type.Name);

            return path;
        }

        #region CRUD

        /// <summary>
        /// 增加一个<see cref="Document"/>到数据库。这实际上创建了一个文件。
        /// <para>Create a new <see cref="Document"/> into database. This operation will create a new file.</para>
        /// </summary>
        /// <param name="item"></param>
        public virtual void Create(Document item)
        {
            if (item.Id != Guid.Empty)
            {
                throw new ArgumentException(string.Format("item({0}) is already created! Try to use Update() instead.", item.Id), "item");
            }

            // 为item创建Id。
            // Create Id for item.
            item.Id = Guid.NewGuid();

            string fullname = GenerateFileFullPath(item);

            System.IO.FileInfo info = new System.IO.FileInfo(fullname);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);

            this.persistence.Serialize(item, fullname);
        }

        /// <summary>
        /// 检索符合给定条件的所有<paramref name="TDocument"/>。
        /// <para>Retrives all <paramref name="TDocument"/> that satisfies the specified condition.</para>
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="predicate">检索出的对象应满足的条件。<para>THe condition that should be satisfied by retrived object.</para></param>
        /// <returns></returns>
        public virtual IList<TDocument> Retrieve<TDocument>(Predicate<TDocument> predicate)
            where TDocument : Document
        {
            IList<TDocument> result = new List<TDocument>();

            if (predicate != null)
            {
                string path = GenerateFilePath(typeof(TDocument));

                if (System.IO.Directory.Exists(path))
                {
                    string extension = this.persistence.Extension;
                    string[] files = System.IO.Directory.GetFiles(
                        path, "*." + extension, SearchOption.AllDirectories);

                    foreach (var fullname in files)
                    {
                        TDocument deserializedFileObject =
                            this.persistence.Deserialize<TDocument>(fullname);

                        if (predicate(deserializedFileObject))
                        {
                            result.Add(deserializedFileObject);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 更新给定的对象。
        /// <para>Update specified <paramref name="item"/>.</para>
        /// </summary>
        /// <param name="item">要被更新的对象。<para>The object to be updated.</para></param>
        public virtual void Update(Document item)
        {
            string fullname = GenerateFileFullPath(item);

            this.persistence.Serialize(item, fullname);
        }

        /// <summary>
        /// 删除指定的对象。
        /// <para>Delete specified <paramref name="item"/>.</para>
        /// </summary>
        /// <param name="item">要被删除的对象。<para>The object to be deleted.</para></param>
        public virtual void Delete(Document item)
        {
            string fullname = GenerateFileFullPath(item);

            File.Delete(fullname);
        }

        public virtual void Delete<TDocument>(Predicate<TDocument> predicate) where TDocument : Document
        {
            string path = GenerateFilePath(typeof(TDocument));
            string[] files = System.IO.Directory.GetFiles(
                path, "*." + this.persistence.Extension, SearchOption.AllDirectories);

            foreach (var fullname in files)
            {
                TDocument deserializedFileObject =
                    this.persistence.Deserialize<TDocument>(fullname);

                if (predicate(deserializedFileObject))
                {
                    File.Delete(fullname);
                }
            }
        }

        #endregion CRUD

    }
}
