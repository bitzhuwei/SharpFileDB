using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFileDB.Blocks;

namespace SharpFileDB
{
    /// <summary>
    /// 单文件数据库上下文，代表一个单文件数据库。SharpFileDB的核心类型。
    /// </summary>
    public class FileDBContext
    {
        /// <summary>
        /// 单文件数据库上下文，代表一个单文件数据库。SharpFileDB的核心类型。
        /// </summary>
        /// <param name="fullname">数据库文件据对路径。</param>
        public FileDBContext(string fullname)
        {
            this.Fullname = fullname;

            if (!File.Exists(fullname))
            {
                CreateDB(fullname);
            }

            InitializeDB(fullname);
        }

        /// <summary>
        /// 根据数据库文件初始化<see cref="FileDBContext"/>。
        /// </summary>
        /// <param name="fullname">数据库文件据对路径。</param>
        private void InitializeDB(string fullname)
        {
            using (FileStream fs = new FileStream(fullname, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096))
            {
                Blocks.HeaderBlock headerBlock = new Blocks.HeaderBlock();
                byte[] bytes = headerBlock.ToBytes();
                fs.Write(bytes, 0, bytes.Length);
                byte[] leftSpace = new byte[4096 - bytes.Length];
                fs.Write(leftSpace, 0, leftSpace.Length);
            }
        }

        /// <summary>
        /// 创建初始状态的数据库文件。
        /// </summary>
        /// <param name="fullname">数据库文件据对路径。</param>
        private void CreateDB(string fullname)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 向数据库新增一条记录。
        /// </summary>
        /// <param name="document"></param>
        public void Insert(Table document)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新数据库内的一条记录。
        /// </summary>
        /// <param name="document"></param>
        public void Update(Table document)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除数据库内的一条记录。
        /// </summary>
        /// <param name="document"></param>
        public void Delete(Table document)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 查找数据库内的某些记录。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">符合此条件的记录会被取出。</param>
        /// <returns></returns>
        public IList<T> Find<T>(object predicate) where T : Table, new()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 数据库文件据对路径。 
        /// </summary>
        public string Fullname { get; set; }
    }
}
