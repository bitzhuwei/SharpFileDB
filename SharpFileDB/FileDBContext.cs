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
        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        TableManager tableManager = new TableManager();
        FreeSpaceManager freeSpaceManager = new FreeSpaceManager();

        /// <summary>
        /// 文件数据库。
        /// <para>Represents a file database.</para>
        /// </summary>
        /// <param name="fullname">数据库文件绝对路径。<para>Database file's fullname.</para></param>
        /// <param name="persistence">持久化方式。<para>The <see cref="IPersistence"/> instance.</para></param>
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
                InitializeTableDict(fs);

                InitializeFreeSpaceDict(fs);
            }
        }

        private void InitializeFreeSpaceDict(FileStream fs)
        {
            object obj = null;
            fs.Seek(0, SeekOrigin.Begin);
            obj = formatter.Deserialize(fs);
            obj = formatter.Deserialize(fs);
            FreeSpaceNode headNode = obj as FreeSpaceNode;

            FreeSpaceNode currentNode = headNode;
            // FreeSpaceNode 在数据库中有头结点。头结点是不存储实际数据的，只保存下一结点。
            // FreeSpaceNode has a head note in database file. Head node stores only next node's position.
            while (currentNode.NextNodePosition != 0)
            {
                fs.Seek(currentNode.NextNodePosition, SeekOrigin.Begin);
                obj = formatter.Deserialize(fs);
                FreeSpaceNode freeSpaceNode = obj as FreeSpaceNode;
                this.freeSpaceManager.freeSpaceNodeList.Add(freeSpaceNode);

                currentNode = freeSpaceNode;
            }
        }

        private void InitializeTableDict(FileStream fs)
        {
            object obj = null;
            fs.Seek(0, SeekOrigin.Begin);
            obj = formatter.Deserialize(fs);
            TableNode headNode = obj as TableNode;
            TableNode currentNode = headNode;
            // TableNode 在数据库中有头结点。头结点是不存储实际数据的，只保存下一结点。
            // TableNode has a head note in database file. Head node stores only next node's position.
            while (currentNode.NextNodePosition != 0)
            {
                TableNode nextNode;
                DocumentNode docNode;
                Type type;
                {
                    fs.Seek(currentNode.NextNodePosition, SeekOrigin.Begin);
                    obj = formatter.Deserialize(fs);
                    nextNode = obj as TableNode;
                }
                {
                    fs.Seek(nextNode.DocumentNodePosition, SeekOrigin.Begin);
                    obj = formatter.Deserialize(fs);
                    docNode = obj as DocumentNode;
                }
                {
                    fs.Seek(docNode.DocumentPosition, SeekOrigin.Begin);
                    obj = formatter.Deserialize(fs);
                    Document doc = obj as Document;
                    type = doc.GetType();
                }

                this.tableManager.tableDict.Add(type, docNode.NextNodePosition);

                currentNode = nextNode;
            }
        }

        private void CreateDB(string fullname)
        {
            TableNode tableHead = new TableNode();
            FreeSpaceNode freeSpaceHead = new FreeSpaceNode();
            FreeSpaceNode freeSpaceNode = new FreeSpaceNode();
            
            using (MemoryStream ms = new MemoryStream())
            {
                // table链表的头结点。
                // head node of table list.
                formatter.Serialize(ms, tableHead);
                long tableHeadNodeLength = ms.Length;

                // 空闲空间链表的头结点。
                // head node of free space list.
                formatter.Serialize(ms, freeSpaceHead);
                long freeSpaceHeadLength = ms.Length - tableHeadNodeLength;
                freeSpaceHead.NextNodePosition = tableHeadNodeLength + freeSpaceHeadLength;
                freeSpaceHead.Position = 0;
                freeSpaceHead.Length = 0;

                // 空闲空间链表的第一个数据结点。
                // first node of free space list that contains data.
                formatter.Serialize(ms, freeSpaceNode);
                long headerLength = ms.Length;
                freeSpaceNode.NextNodePosition = 0;
                freeSpaceNode.Position = headerLength;// 空闲空间起始位置。Start position of the only free space.
                freeSpaceNode.Length = long.MaxValue - headerLength;// 空闲空间长度。Length of the only free space. 
            }

            using (FileStream fs = System.IO.File.Create(fullname))
            {
                formatter.Serialize(fs, tableHead);
                formatter.Serialize(fs, freeSpaceHead);
                formatter.Serialize(fs, freeSpaceNode);
            }
        }

        public override string ToString()
        {
            //return string.Format("@: {0}, IPersistence: {1}", Fullname, persistence);
            return string.Format("@: {0}", Fullname);
        }

        #region Properties

        /// <summary>
        /// 数据库文件所在目录
        /// <para>Directory of database files.</para>
        /// </summary>
        public virtual string Fullname { get; protected set; }

        ///// <summary>
        ///// 文件数据库使用此接口进行持久化相关的操作。
        ///// <para>FIle database executes persistence operations via this interface.</para>
        ///// </summary>
        //public virtual IPersistence persistence { get; set; }

        #endregion


        //protected string GenerateFileFullPath(Document item)
        //{
        //    Type fileObjectType = item.GetType();
        //    string path = GenerateFilePath(fileObjectType);

        //    string name = item.GenerateFileName(this.persistence.Extension);

        //    string fullname = Path.Combine(path, name);

        //    return fullname;
        //}

        ///// <summary>
        ///// 生成文件路径
        ///// </summary>
        ///// <typeparam name="TDocument">文档类型</typeparam>
        ///// <returns>文件路径</returns>
        //protected string GenerateFilePath(Type type)
        //{
        //    string path = Path.Combine(this.Fullname, type.Name);

        //    return path;
        //}

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

            // TODO: 锁住这个数据库。
            Transaction transaction = new Transaction();

            Type type = item.GetType();
            if(!this.tableManager.tableDict.ContainsKey(type))
            {
                TableNode tableNode = new TableNode();
                int maxLevel = this.tableManager.tableDict.MaxListLevel;
                DocumentNode[] docNodes = new DocumentNode[maxLevel];
                docNodes[0] = new DocumentNode();
                for (int i = 1; i < maxLevel; i++)
                {
                    docNodes[i] = new DocumentNode();
                }
                long docNodeSpace = docNodes[0].Serialize().Length;

                
                TableNode newTable = new TableNode();
                
            }
            //this.persistence.Serialize(item, fullname);
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
                //string path = GenerateFilePath(typeof(TDocument));

                //if (System.IO.Directory.Exists(path))
                //{
                //    string extension = this.persistence.Extension;
                //    string[] files = System.IO.Directory.GetFiles(
                //        path, "*." + extension, SearchOption.AllDirectories);

                //    foreach (var fullname in files)
                //    {
                //        TDocument deserializedFileObject =
                //            this.persistence.Deserialize<TDocument>(fullname);

                //        if (predicate(deserializedFileObject))
                //        {
                //            result.Add(deserializedFileObject);
                //        }
                //    }
                //}
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
            //string fullname = GenerateFileFullPath(item);

            //this.persistence.Serialize(item, fullname);
        }

        /// <summary>
        /// 删除指定的对象。
        /// <para>Delete specified <paramref name="item"/>.</para>
        /// </summary>
        /// <param name="item">要被删除的对象。<para>The object to be deleted.</para></param>
        public virtual void Delete(Document item)
        {
            throw new NotImplementedException();
            //string fullname = GenerateFileFullPath(item);

            //File.Delete(fullname);
        }

        public virtual void Delete<TDocument>(Predicate<TDocument> predicate) where TDocument : Document
        {
            throw new NotImplementedException();
            //string path = GenerateFilePath(typeof(TDocument));
            //string[] files = System.IO.Directory.GetFiles(
            //    path, "*." + this.persistence.Extension, SearchOption.AllDirectories);

            //foreach (var fullname in files)
            //{
            //    TDocument deserializedFileObject =
            //        this.persistence.Deserialize<TDocument>(fullname);

            //    if (predicate(deserializedFileObject))
            //    {
            //        File.Delete(fullname);
            //    }
            //}
        }

        #endregion CRUD

    }
}
