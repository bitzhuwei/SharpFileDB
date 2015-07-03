using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Services
{
    /// <summary>
    /// 为数据库文件提供关于页的操作。
    /// </summary>
    public static class DiskService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static long Alloc(this FileDBContext db, int length)
        {
            FileStream fs = db.FileStream;
            Blocks.DBHeaderBlock headerBlock = db.HeaderBlock;

            throw new NotImplementedException();
        }
    }
}
