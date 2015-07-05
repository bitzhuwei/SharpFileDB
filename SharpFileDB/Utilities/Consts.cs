using SharpFileDB.Blocks;
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
    /// 文件数据库所用各项常量。
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
        /// <see cref="DataBlock.Data"/>的最大长度。
        /// 据上一次测试（2015-07-05_15-05-00）此值为3925 = <see cref="Consts.pageSize"/> - 175。
        /// </summary>
        public static readonly Int16 maxDataBytes;

        /// <summary>
        /// 系统启动时初始化一个页内可用的最大空间（字节数）。
        /// </summary>
        static Consts()
        {
            {
                PageHeaderBlock block = new PageHeaderBlock();
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, block);
                    if (ms.Length > Consts.pageSize / 10)
                    { throw new Exception("Page header block takes too much space!"); }
                    Consts.maxAvailableSpaceInPage = (Int16)(Consts.pageSize - ms.Length);
                }
            }
            {
                DataBlock block = new DataBlock();
                block.Data = new byte[0];
                Int16 minValue;
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, block);
                    if (ms.Length > Consts.pageSize / 10)
                    { throw new Exception("data block's metadata takes too much space!"); }
                    minValue = (Int16)ms.Length;
                }
                Consts.maxDataBytes = (Int16)(Consts.pageSize - minValue);
            }
        }

    }
}
