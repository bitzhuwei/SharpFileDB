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

        class TypeComparer : IComparer<Type>
        {

            #region IComparer<Type> 成员

            public int Compare(Type x, Type y)
            {
                if (x == null)
                {
                    if (y == null)
                    { return 0; }
                    else
                    { return -1; }
                }
                else
                {
                    if (y == null)
                    { return 1; }
                    else
                    {
                        int xCode = x.GetHashCode();
                        int yCode = y.GetHashCode();
                        int result = xCode = yCode;
                        return result;
                    }
                }
            }

            #endregion
        }

        const int maxLevel = 32;
        SkipList<Type, long> tableDict = new SkipList<Type, long>(maxLevel, 0.5f, new TypeComparer());

        //SkipList<long, long> freeBytesDict = new SkipList<long, long>(maxLevel, 0.5f, Comparer<long>.Default);
        //List<long> freeSpacePositionList = new List<long>();
        //List<long> freeSpaceLengthList = new List<long>();
        //List<long> freeSpaceNextList = new List<long>();
        List<FreeSpaceNode> freeSpaceNodeList = new List<FreeSpaceNode>();

        /// <summary>
        /// 文件数据库。
        /// <para>Represents a file database.</para>
        /// </summary>
        /// <param name="fullname">数据库文件绝对路径。<para>Database file's fullname.</para></param>
        /// <param name="persistence">持久化方式。<para>The <see cref="IPersistence"/> instance.</para></param>
        public FileDBContext(string fullname = null, IPersistence persistence = null)
        {
            if (fullname == null)
            {
                this.Fullname = Path.Combine(Environment.CurrentDirectory, "SharpFileDB", "SharpFile.db");
            }
            else
            {
                Fullname = fullname;
            }

            if (persistence == null)
            {
                this.persistence = new DefaultPersistence();
            }
            else
            {
                this.persistence = persistence;
            }


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
            TableNode tableNodeHead = obj as TableNode;

            long freeBytesNodeHeadPosition = fs.Position;
            fs.Seek(freeBytesNodeHeadPosition, SeekOrigin.Begin);
            FreeSpaceNode freeBytesNodeHead = obj as FreeSpaceNode;

            FreeSpaceNode currentFreeBytesNode = freeBytesNodeHead;
            // FreeBytesNode 在数据库中没有头结点。
            // FreeBytesNode has no head note in database file.
            while (currentFreeBytesNode != null)
            {
                freeSpaceNodeList.Add(currentFreeBytesNode);

                if (currentFreeBytesNode.NextNodePosition != 0)
                {
                    fs.Seek(currentFreeBytesNode.NextNodePosition, SeekOrigin.Begin);
                    obj = formatter.Deserialize(fs);
                    currentFreeBytesNode = obj as FreeSpaceNode;
                }
                else
                { currentFreeBytesNode = null; }
            }
        }

        private void InitializeTableDict(FileStream fs)
        {
            object obj = null;
            fs.Seek(0, SeekOrigin.Begin);
            obj = formatter.Deserialize(fs);
            TableNode tableNodeHead = obj as TableNode;
            long freeBytesNodeHeadPosition = fs.Position;
            TableNode currentTableNode = tableNodeHead;
            // TableNode 在数据库中有头结点。头结点是不存储实际数据的，只保存下一结点。
            // TableNode has a head note in database file. Head node stores only next node's position.
            while (currentTableNode.NextNodePosition != 0)
            {
                TableNode nextNode;
                DocumentNode docNode;
                Type type;
                {
                    fs.Seek(currentTableNode.NextNodePosition, SeekOrigin.Begin);
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

                this.tableDict.Add(type, docNode.NextNodePosition);

                currentTableNode = nextNode;
            }
        }

        private void CreateDB(string fullname)
        {
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                TableNode tableNode = new TableNode();
                formatter.Serialize(ms, tableNode);
                long tableNodeLength = ms.Length;

                FreeSpaceNode freeBytesNode = new FreeSpaceNode();
                formatter.Serialize(ms, freeBytesNode);
                freeBytesNode.Position = ms.Length;
                freeBytesNode.Length = long.MaxValue - ms.Length;

                ms.Seek(tableNodeLength, SeekOrigin.Begin);
                formatter.Serialize(ms, freeBytesNode);

                bytes = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(bytes, 0, bytes.Length);
            }
            using (FileStream fs = System.IO.File.Create(fullname))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        public override string ToString()
        {
            return string.Format("@: {0}, IPersistence: {1}", Fullname, persistence);
        }

        #region Properties

        /// <summary>
        /// 数据库文件所在目录
        /// <para>Directory of database files.</para>
        /// </summary>
        public virtual string Fullname { get; protected set; }

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
            string path = Path.Combine(this.Fullname, type.Name);

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
