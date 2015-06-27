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
        /// 在数据库文件开头写入一些不能被IFormatter反序列化的字节。这样有助于发现bug。
        /// <para>Write some bytes that cannot be deserialized by IFormatter. This helps to find bugs.</para>
        /// </summary>
        const int firstPositionInFile = 1;

        /// <summary>
        /// 序列化/反序列化工具。
        /// <para>Serializer/Deserializer.</para>
        /// </summary>
        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        /// <summary>
        /// 管理数据库的所有Table。
        /// <para>Manages all tables in this database.</para>
        /// </summary>
        TableManager tableManager = new TableManager();
        
        /// <summary>
        /// 管理数据库的所有空闲空间。
        /// <para>Manages all free spaces in this database.</para>
        /// </summary>
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
            fs.Seek(firstPositionInFile, SeekOrigin.Begin);
            obj = formatter.Deserialize(fs);
            long headNodePosition = fs.Position;
            obj = formatter.Deserialize(fs);
            FreeSpaceNode headNode = obj as FreeSpaceNode;
            headNode.SerializedPositionInFile = headNodePosition;
            headNode.SerializedLengthInFile = fs.Position - headNodePosition;

            FreeSpaceNode currentNode = headNode;
            // FreeSpaceNode 在数据库中有头结点。头结点是不存储实际数据的，只保存下一结点。
            // FreeSpaceNode has a head note in database file. Head node stores only next node's position.
            while (currentNode.NextSerializedPositionInFile != 0)
            {
                FreeSpaceNode freeSpaceNode = currentNode.GetNext(fs) as FreeSpaceNode;
                this.freeSpaceManager.freeSpaceNodeList.Add(freeSpaceNode);

                currentNode = freeSpaceNode;
            }
        }

        private void InitializeTableDict(FileStream fs)
        {
            object obj = null;
            fs.Seek(firstPositionInFile, SeekOrigin.Begin);
            obj = formatter.Deserialize(fs);
            TableNode headNode = obj as TableNode;
            headNode.SerializedPositionInFile = firstPositionInFile;

            TableNode currentNode = headNode;
            // TableNode 在数据库中有头结点。头结点是不存储实际数据的，只保存下一结点。
            // TableNode has a head note in database file. Head node stores only next node's position.
            while (currentNode.NextSerializedPositionInFile != 0)
            {
                TableNode nextNode;
                DocumentNode docNode;
                Type type;
                {
                    nextNode = currentNode.GetNext(fs) as TableNode;
                }
                {
                    fs.Seek(nextNode.DocumentNodePosition, SeekOrigin.Begin);
                    obj = formatter.Deserialize(fs);
                    docNode = obj as DocumentNode;
                    docNode.SerializedPositionInFile = nextNode.DocumentNodePosition;
                }
                {
                    fs.Seek(docNode.DocumentPosition, SeekOrigin.Begin);
                    obj = formatter.Deserialize(fs);
                    Document doc = obj as Document;
                    doc.SerializedPositionInFile = docNode.DocumentPosition;
                    type = doc.GetType();
                }

                this.tableManager.tableDict.Add(type, docNode.NextSerializedPositionInFile);

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
                freeSpaceHead.SpaceLength = 0;
                freeSpaceHead.StartPosition = 0;
                freeSpaceHead.SerializedPositionInFile = firstPositionInFile + tableHeadNodeLength;
                freeSpaceHead.SerializedLengthInFile = freeSpaceHeadLength;
                freeSpaceHead.NextSerializedPositionInFile = firstPositionInFile + tableHeadNodeLength + freeSpaceHeadLength;

                // 空闲空间链表的第一个数据结点。
                // first node of free space list that contains data.
                formatter.Serialize(ms, freeSpaceNode);
                long headerLength = ms.Length;
                freeSpaceNode.SpaceLength = long.MaxValue - (firstPositionInFile + headerLength);// 空闲空间长度。Length of the only free space. 
                freeSpaceNode.StartPosition = firstPositionInFile + headerLength;// 空闲空间起始位置。Start position of the only free space.
                freeSpaceNode.SerializedPositionInFile = firstPositionInFile + tableHeadNodeLength + freeSpaceHeadLength;
                freeSpaceNode.SerializedLengthInFile = freeSpaceHeadLength;
                freeSpaceNode.NextSerializedPositionInFile = 0;
            }

            using (FileStream fs = System.IO.File.Create(fullname))
            {
                for (int i = 0; i < firstPositionInFile; i++)
                {
                    fs.Write(new byte[] { 0xAA }, 0, firstPositionInFile);
                }
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

            // TODO: 锁住这个数据库。
            Transaction transaction = new Transaction();

            Type type = item.GetType();
            if (!this.tableManager.tableDict.ContainsKey(type))
            {
                CreateTableForType(type, transaction);
            }

            using (FileStream fs = new FileStream(Fullname, FileMode.Open, FileAccess.ReadWrite))
            {
                transaction.Commit(fs);
            }
        }

        private void CreateTableForType(Type type, Transaction transaction)
        {
            // 创建 table node.
            TableNode tableNode = new TableNode();
            // 创建 document node.
            //DocumentNode level0Node = new DocumentNode();
            int maxLevel = this.tableManager.tableDict.MaxListLevel;
            DocumentNode[] docNodes = new DocumentNode[maxLevel];
            for (int i = 0; i < maxLevel; i++)
            {
                docNodes[i] = new DocumentNode();
            }
            // 分配空间。
            {
                DocumentNode level0Node = docNodes[0];
                long docNodeLength = level0Node.Serialize().LongLength;
                for (int i = 1; i < maxLevel; i++)
                {
                    // 分配下面的 document node.
                    long pos = this.freeSpaceManager.AllocateFreeSpace(docNodeLength, transaction);
                    DocumentNode node = docNodes[i - 1];
                    node.SerializedPositionInFile = pos;
                    docNodes[i].NextSerializedPositionInFile = pos;
                    //transaction.Add(node, node.PositionInFile);
                    transaction.Add(node);
                }

                {
                    // 分配最上面的 docment node.
                    long pos = this.freeSpaceManager.AllocateFreeSpace(docNodeLength, transaction);
                    DocumentNode node = docNodes[maxLevel - 1];
                    node.SerializedPositionInFile = pos;
                    tableNode.DocumentNodePosition = node.SerializedPositionInFile;
                    //transaction.Add(node, node.PositionInFile);
                    transaction.Add(node);
                }
            }
            {
                // 分配新建的Table.
                long tableNodeLength = tableNode.Serialize().LongLength;
                long pos = this.freeSpaceManager.AllocateFreeSpace(tableNodeLength, transaction);
                tableNode.SerializedPositionInFile = pos;
                //transaction.Add(tableNode, tableNode.PositionInFile);
                transaction.Add(tableNode);
            }

            {
                // 更新上一个Table的指针。
                TableNode lastTableNode = this.tableManager.LastTableNode();
                //long lastTableNodeLength = lastTableNode.Serialize().LongLength;
                lastTableNode.NextSerializedPositionInFile = tableNode.SerializedPositionInFile;
                //transaction.Add(lastTableNode, lastTableNode.PositionInFile);
                transaction.Add(lastTableNode);
            }

            this.tableManager.tableDict.Add(type, tableNode.DocumentNodePosition);
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
