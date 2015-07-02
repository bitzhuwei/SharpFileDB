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
        //Dictionary<Type, Table> tableManager = new Dictionary<Type, Table>();
        //SkipList<Type, Table> tableManager = new SkipList<Type, Table>(32, 0.5f, Comparer<Type>.Default);

        Services.CacheService dirtyPageCache;// = new Services.CacheService();

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

            InitializeDB(fullname);
        }

        private void InitializeDB(string fullname)
        {
            Services.DiskService disk = new Services.DiskService(fullname);
            this.DiskService = disk;

            using (FileStream fs = new FileStream(fullname, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // 读取数据库header
                    Pages.DBHeaderPage headerPage = new Pages.DBHeaderPage();
                    headerPage.ReadHeader(br);
                    headerPage.ReadContent(br);
                    this.headerPage = headerPage;

                    // 读取数据库所有Table
                    Pages.TablePage tableHead = new Pages.TablePage();
                    tableHead.pageHeaderInfo.pageID = headerPage.FirstTablePageID;
                    tableHead.ReadHeader(br);
                    tableHead.ReadContent(br);

                    Pages.TablePage currentTable = tableHead;
                    while (currentTable.pageHeaderInfo.nextPageID != long.MaxValue)
                    {
                        long nextPageID = currentTable.pageHeaderInfo.nextPageID;
                        Pages.TablePage tablePage = disk.ReadPage<Pages.TablePage>(nextPageID);
                        tablePage.pageHeaderInfo.pageID = nextPageID;
                    }
                }
            }
        }

        private void CreateDB(string fullname)
        {
            // 创建数据库文件。
            using (FileStream fs = new FileStream(fullname, FileMode.CreateNew, FileAccess.Write))
            {
                // 使用BinaryWriter。
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    // 写入数据库自身的头信息。
                    Pages.DBHeaderPage headerPage = new Pages.DBHeaderPage();
                    headerPage.FirstTablePageID = 1;
                    headerPage.WriteHeader(bw);
                    headerPage.WriteContent(bw);
                    bw.Write(new byte[Pages.PageAddress.PAGE_SIZE - fs.Position]);

                    // 写入Table的头结点。头结点不保存实际数据，是为编码方便而设置的。
                    Pages.TablePage tableHead = new Pages.TablePage();
                    tableHead.WriteHeader(bw);
                    tableHead.WriteContent(bw);
                    bw.Write(new byte[Pages.PageAddress.PAGE_SIZE * 2 - fs.Position]);
                }
                // 初始数据库文件大小应该为8 KB = 2 Pages.
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


        public Pages.DBHeaderPage headerPage { get; set; }

        public Services.DiskService DiskService { get; set; }
    }
}
