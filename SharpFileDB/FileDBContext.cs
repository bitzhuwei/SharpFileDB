using NGenerics.DataStructures;
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
        /// 序列化/反序列化工具。
        /// <para>Serializer/Deserializer.</para>
        /// </summary>
        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        /// <summary>
        /// 管理数据库的所有Table。
        /// <para>Manages all tables in this database.</para>
        /// </summary>
        Dictionary<Type, Table> tableManager = new Dictionary<Type, Table>();
        //SkipList<Type, Table> tableManager = new SkipList<Type, Table>(32, 0.5f, Comparer<Type>.Default);
        


        /// <summary>
        /// 文件数据库。
        /// <para>Represents a file database.</para>
        /// </summary>
        /// <param name="fullname">数据库文件绝对路径。<para>Database file's fullname.</para></param>
        ///// <param name="persistence">持久化方式。<para>The <see cref="IPersistence"/> instance.</para></param>
        public FileDBContext(string fullname = null)//, IPersistence persistence = null)
        {
            if (fullname == null)
            {
                this.Fullname = Path.Combine(Environment.CurrentDirectory, "SharpFileDB", "SharpFile.db");
            }
            else
            {
                Fullname = fullname;
            }

            //if (persistence == null)
            //{
            //    this.persistence = new DefaultPersistence();
            //}
            //else
            //{
            //    this.persistence = persistence;
            //}


            if (!File.Exists(fullname))
            {
                CreateDB(fullname);
            }
            else
            {
                InitializeDB(fullname);
            }
        }

        private void InitializeDB(string fullname)
        {
            using (FileStream fs = new FileStream(fullname, FileMode.Open, FileAccess.Read))
            {
            }
        }

        private void CreateDB(string fullname)
        {
            using (FileStream fs = new FileStream(fullname, FileMode.CreateNew, FileAccess.Write))
            {
                // TODO: create database file.

            }
        }

        public override string ToString()
        {
            //return string.Format("@: {0}, IPersistence: {1}", Fullname, persistence);
            return string.Format("@: {0}", Fullname);
        }

        /// <summary>
        /// 数据库文件所在目录
        /// <para>Directory of database files.</para>
        /// </summary>
        public virtual string Fullname { get; protected set; }

        #region CRUD

        /// <summary>
        /// 增加一个<see cref="Document"/>到数据库。这实际上创建了一个文件。
        /// <para>Insert a new <see cref="Document"/> into database. This operation will create a new file.</para>
        /// </summary>
        /// <param name="item"></param>
        public virtual void Insert(Document item)
        {
            if (item.Id != Guid.Empty)
            {
                throw new ArgumentException(string.Format("item({0}) is already created! Try to use Update() instead.", item.Id), "item");
            }

            // 为item创建Id。
            // Create Id for item.
            item.Id = Guid.NewGuid();

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
                throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除指定的对象。
        /// <para>Delete specified <paramref name="item"/>.</para>
        /// </summary>
        /// <param name="item">要被删除的对象。<para>The object to be deleted.</para></param>
        public virtual void Delete(Document item)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete<TDocument>(Predicate<TDocument> predicate) where TDocument : Document
        {
            throw new NotImplementedException();
        }

        #endregion CRUD

    }
}
