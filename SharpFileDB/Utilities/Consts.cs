using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
//using System.Threading.Tasks;

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
        /// 一个页内可用的最大空间（字节数）。是去掉<see cref="PageHeaderBlock"/>后剩余的字节数。
        /// </summary>
        public static readonly Int16 maxAvailableSpaceInPage;

        /// <summary>
        /// 
        /// </summary>
        public static readonly Int16 minOccupiedBytes;

        /// <summary>
        /// <see cref="DataBlock.Data"/>的最大长度。
        /// 据上一次测试（2015-07-06_13-41-00）此值为3763 = <see cref="Consts.pageSize"/> - 333。
        /// </summary>
        public static readonly Int16 maxDataBytes;

        /// <summary>
        /// <see cref="Table.Id"/>这一属性的名字。
        /// </summary>
        public static string TableIdString;

        /// <summary>
        /// <see cref="DBHeaderBlock"/>序列化后的长度。
        /// </summary>
        public static Int16 dbHeaderBlockLength;

        /// <summary>
        /// <see cref="PageHeaderBlock"/>序列化后的长度。
        /// </summary>
        public static Int16 pageHeaderBlockLength;

        /// <summary>
        /// <see cref="TableBlock"/>序列化后的长度。
        /// </summary>
        public static Int16 tableBlockLength;

        /// <summary>
        /// <see cref="IndexBlock"/>序列化后的长度。
        /// </summary>
        public static Int16 indexBlockLength;

        /// <summary>
        /// <see cref="SkipListNodeBlock"/>序列化后的长度。
        /// </summary>
        public static Int16 skipListNodeBlockLength;

        /// <summary>
        /// <see cref="DataBlock"/>序列化后的长度。
        /// </summary>
        public static Int16 dataBlockLength;

        /// <summary>
        /// 系统启动时初始化各项常量。
        /// </summary>
        static Consts()
        {
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
                PageHeaderBlock page = new PageHeaderBlock();
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, page);
                    if (ms.Length > Consts.pageSize / 10)
                    { throw new Exception("Page header block takes too much space!"); }
                    Consts.pageHeaderBlockLength = (Int16)ms.Length;
                    Consts.maxAvailableSpaceInPage = (Int16)(Consts.pageSize - ms.Length);
                    Consts.minOccupiedBytes = (Int16)(Consts.pageSize - Consts.maxAvailableSpaceInPage);
                }
                BlockCache.TryRemoveFloatingBlock(page);
            }


            {
                PageHeaderBlock page = new PageHeaderBlock();
                DBHeaderBlock dbHeader = new DBHeaderBlock();
                TableBlock tableHead = new TableBlock();
                Int16 usedSpaceInFirstPage;
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, page);
                    dbHeader.ThisPos = ms.Length;
                    formatter.Serialize(ms, dbHeader);
                    tableHead.ThisPos = ms.Length;
                    Consts.dbHeaderBlockLength = (Int16)(ms.Length - Consts.pageHeaderBlockLength);
                    formatter.Serialize(ms, tableHead);
                    Consts.tableBlockLength = (Int16)(ms.Length - tableHead.ThisPos);
                    usedSpaceInFirstPage = (Int16)ms.Length;
                }

                if (usedSpaceInFirstPage > Consts.pageSize)
                {
                    throw new Exception("First page is full!");
                }
                BlockCache.TryRemoveFloatingBlock(page);
                BlockCache.TryRemoveFloatingBlock(dbHeader);
                BlockCache.TryRemoveFloatingBlock(tableHead);
            }
            {
                IndexBlock block = new IndexBlock();
                int length = block.ToBytes().Length;
                if (length > Consts.pageSize / 10)
                { throw new Exception("index block takes too much space!"); }
                Consts.indexBlockLength = (Int16)length;
                BlockCache.TryRemoveFloatingBlock(block);
            }
            {
                SkipListNodeBlock block = new SkipListNodeBlock();
                int length = block.ToBytes().Length;
                if (length > Consts.pageSize / 10)
                { throw new Exception("index block takes too much space!"); }
                Consts.skipListNodeBlockLength = (Int16)length;
                BlockCache.TryRemoveFloatingBlock(block);
            }
            {
                PageHeaderBlock page = new PageHeaderBlock();
                DataBlock dataBlock = new DataBlock();
                dataBlock.Data = new byte[0];
                Int16 minValue;
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, page);
                    long pos = ms.Length;
                    formatter.Serialize(ms, dataBlock);
                    Consts.dataBlockLength = (Int16)(ms.Length - pos);
                    if (ms.Length > Consts.pageSize / 10)
                    { throw new Exception("data block's metadata takes too much space!"); }
                    minValue = (Int16)ms.Length;
                }
                Consts.maxDataBytes = (Int16)(Consts.pageSize - minValue);
                BlockCache.TryRemoveFloatingBlock(page);
                BlockCache.TryRemoveFloatingBlock(dataBlock);
            }
        }

    }
}
