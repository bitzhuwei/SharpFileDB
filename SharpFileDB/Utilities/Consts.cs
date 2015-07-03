using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// 文件数据库所用各项常量、公共变量。
    /// </summary>
    public class Consts
    {
        /// <summary>
        /// 序列化/反序列化工具。
        /// </summary>
        public static readonly BinaryFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// 一页的大小。4KB
        /// </summary>
        public const Int16 pageSize = 4096;// 4KB

        /// <summary>
        /// 一个页内可用的最大空间（字节数）。
        /// </summary>
        public static readonly Int16 maxAvailableSpaceInPage;

        /// <summary>
        /// 系统启动时初始化一个页内可用的最大空间（字节数）。
        /// </summary>
        static Consts()
        {
            Blocks.PageHeaderBlock block = new Blocks.PageHeaderBlock();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, block);
                if (ms.Length > 4096 / 2)
                { throw new Exception("Page header block takes too much space!"); }
                maxAvailableSpaceInPage = (Int16)(4096 - ms.Length);
            }
        }

    }
}
