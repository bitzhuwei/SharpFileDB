using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFileDB.Blocks;
using SharpFileDB.Utilities;
using System.Reflection;
using System.Linq.Expressions;

namespace SharpFileDB
{
    /// <summary>
    /// 单文件数据库上下文，代表一个单文件数据库。SharpFileDB的核心类型。
    /// </summary>
    public partial class FileDBContext : IDisposable
    {


        /// <summary>
        /// 更新数据库内的一条记录。
        /// </summary>
        /// <param name="record"></param>
        public void Update(Table record)
        {
            throw new NotImplementedException();
        }

    }
}
