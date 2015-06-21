using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// Represents a file database.
    /// </summary>
    public class FileDBContext
    {
        #region Fields

        /// <summary>
        /// 文件数据库操作锁
        /// </summary>
        protected static readonly object operationLock = new object();
        private static HashSet<char> invalidFileNameChars;

        static FileDBContext()
        {
            invalidFileNameChars = new HashSet<char>() { '\0', ' ', '.', '$', '/', '\\' };
            foreach (var c in Path.GetInvalidPathChars()) { invalidFileNameChars.Add(c); }
            foreach (var c in Path.GetInvalidFileNameChars()) { invalidFileNameChars.Add(c); }
        }

        /// <summary>
        /// 文件数据库
        /// </summary>
        /// <param name="directory">数据库文件所在目录</param>
        public FileDBContext(string directory = null)
        {
            if (directory == null)
            {
                this.Directory = Environment.CurrentDirectory;
            }
            else
            {
                Directory = directory;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}\\*.{1}", Directory, FileExtension);
            //return base.ToString();
        }

        #region Properties

        /// <summary>
        /// 数据库文件所在目录
        /// </summary>
        public virtual string Directory { get; private set; }

        /// <summary>
        /// 是否输出缩进
        /// </summary>
        public virtual bool OutputIndent { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public virtual string FileExtension { get; set; }

        #endregion

        #region CRUD

        public void Create(FileObject item)
        {
            throw new NotImplementedException();
        }

        public IList<TFileObject> Retrieve<TFileObject>(Predicate<TFileObject> pre)
            where TFileObject : FileObject
        {
            IList<TFileObject> result = new List<TFileObject>();
            if (pre != null)
            {
                throw new NotImplementedException();
            }

            return result;
        }



        public void Update(FileObject cat)
        {
            throw new NotImplementedException();
        }

        public void Delete(FileObject cat)
        {
            throw new NotImplementedException();
        }

        #endregion CRUD

    }
}