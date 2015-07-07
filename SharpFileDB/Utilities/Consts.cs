using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// 文件数据库所用各项常量。
    /// </summary>
    internal class Consts
    {

        /// <summary>
        /// 序列化/反序列化工具。
        /// </summary>
        internal static readonly BinaryFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// 一页的大小。4KB
        /// </summary>
        internal const Int16 pageSize = 4096;// 4KB

        /// <summary>
        /// 一个页内可用的最大空间（字节数）。是去掉<see cref="PageHeaderBlock"/>后剩余的字节数。
        /// </summary>
        internal static readonly Int16 maxAvailableSpaceInPage;

        /// <summary>
        /// 
        /// </summary>
        internal static readonly Int16 minOccupiedBytes;

        /// <summary>
        /// <see cref="DataBlock.Data"/>的最大长度。
        /// 据上一次测试（2015-07-06_13-41-00）此值为3763 = <see cref="Consts.pageSize"/> - 333。
        /// </summary>
        internal static readonly Int16 maxDataBytes;

        /// <summary>
        /// <see cref="Table.Id"/>这一属性的名字。
        /// </summary>
        internal static string TableIdString;

        /// <summary>
        /// 系统启动时初始化各项常量。
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
                    Consts.minOccupiedBytes = (Int16)(Consts.pageSize - Consts.maxAvailableSpaceInPage);
                }
            }
            {
                PageHeaderBlock pageHeader = new PageHeaderBlock();
                DataBlock block = new DataBlock();
                block.Data = new byte[0];
                Int16 minValue;
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, pageHeader);
                    formatter.Serialize(ms, block);
                    if (ms.Length > Consts.pageSize / 10)
                    { throw new Exception("data block's metadata takes too much space!"); }
                    minValue = (Int16)ms.Length;
                }
                Consts.maxDataBytes = (Int16)(Consts.pageSize - minValue);
            }
            {
                PropertyInfo[] properties = typeof(Table).GetProperties();
                foreach (var property in properties)
                {
                    TableIndexAttribute attr = property.GetCustomAttribute<TableIndexAttribute>();
                    if (attr != null && property.PropertyType == typeof(ObjectId))
                    {
                        Consts.TableIdString = property.Name;
                        break;
                    }
                }
            }
            {
                PageHeaderBlock pageHeader = new PageHeaderBlock();
                DBHeaderBlock dbHeader = new DBHeaderBlock();
                TableBlock tableBlock = new TableBlock();
                Int16 usedSpaceInFirstPage;
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, pageHeader);
                    dbHeader.ThisPos = ms.Length;
                    formatter.Serialize(ms, dbHeader);
                    tableBlock.ThisPos = ms.Length;
                    formatter.Serialize(ms, tableBlock);
                    usedSpaceInFirstPage = (Int16)ms.Length;
                }

                if (usedSpaceInFirstPage > Consts.pageSize)
                {
                    throw new Exception("First page is full!");
                }
            }
        }

    }
}
