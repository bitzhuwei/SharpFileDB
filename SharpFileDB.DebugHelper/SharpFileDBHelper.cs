using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpFileDB.Blocks;
using SharpFileDB.Utilities;

namespace SharpFileDB.DebugHelper
{
    /// <summary>
    /// 打印<see cref="SharpFileDB"/>的详细信息。
    /// </summary>
    public static class SharpFileDBHelper
    {
        static string Print(SkipListNodeBlock skipListNodeBlock)
        {
            return string.Format("Pos: {0}, K: {1}, V: {2}, Down: {3}, Right: {4}",
                skipListNodeBlock.ThisPos, skipListNodeBlock.KeyPos, skipListNodeBlock.ValuePos,
                skipListNodeBlock.DownPos, skipListNodeBlock.RightPos);
        }

        static string Print(PageHeaderBlock pageHeaderBlock)
        {
            return string.Format("Pos: {0}, available: {1}, Occupied: {2}",
                pageHeaderBlock.ThisPos, pageHeaderBlock.AvailableBytes, pageHeaderBlock.OccupiedBytes);
        }

        /// <summary>
        /// 打印数据库文件里所有表的所有主键的skip list和所有类型的页链表。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static string Print(this FileDBContext db)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(db.Fullname);
            FileStream fs = db.GetFileStream();
            PageHeaderBlock firstPage = fs.ReadBlock<PageHeaderBlock>(0);
            DBHeaderBlock dbHeader = fs.ReadBlock<DBHeaderBlock>(fs.Position);

            PrintSkipLists(db, builder, fs);

            builder.AppendLine();
            builder.AppendLine("Page Info.:");
            for (long i = 0; i < fs.Length; i += Consts.pageSize)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(i);
                string str = Print(pageInfo);
                builder.AppendLine(str);
            }
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("table pages:");
            long tablePos = dbHeader.FirstTablePagePos;
            while (tablePos != 0)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(tablePos);
                builder.Append(string.Format(" {0}[{1}] ->", pageInfo.ThisPos, pageInfo.ThisPos / Consts.pageSize));
                tablePos = pageInfo.NextPagePos;
            }
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("index pages:");
            long indexPos = dbHeader.FirstIndexPagePos;
            while (indexPos != 0)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(indexPos);
                builder.Append(string.Format(" {0}[{1}] ->", pageInfo.ThisPos, pageInfo.ThisPos / Consts.pageSize));
                indexPos = pageInfo.NextPagePos;
            }
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("skip list node pages:");
            long skiplistnodePos = dbHeader.FirstSkipListNodePagePos;
            while (skiplistnodePos != 0)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(skiplistnodePos);
                builder.Append(string.Format(" {0}[{1}] ->", pageInfo.ThisPos, pageInfo.ThisPos / Consts.pageSize));
                skiplistnodePos = pageInfo.NextPagePos;
            }
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("data block pages:");
            long dataBlockPos = dbHeader.FirstDataPagePos;
            while (dataBlockPos != 0)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(dataBlockPos);
                builder.Append(string.Format(" {0}[{1}] ->", pageInfo.ThisPos, pageInfo.ThisPos / Consts.pageSize));
                dataBlockPos = pageInfo.NextPagePos;
            }
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("empty pages:");
            long emptyPos = dbHeader.FirstEmptyPagePos;
            while (emptyPos != 0)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(emptyPos);
                builder.Append(string.Format(" {0}[{1}] ->", pageInfo.ThisPos, pageInfo.ThisPos / Consts.pageSize));
                emptyPos = pageInfo.NextPagePos;
            }
            builder.AppendLine();

            return builder.ToString();
        }

        private static void PrintSkipLists(FileDBContext db, StringBuilder builder, FileStream fs)
        {

            TableBlock tableHead = fs.ReadBlock<TableBlock>(fs.Position);
            TableBlock currentTableBlock = tableHead;
            while (currentTableBlock.NextPos != 0)// 依次Print各个表
            {
                TableBlock tableBlock = fs.ReadBlock<TableBlock>(currentTableBlock.NextPos);
                builder.Append("Table:"); builder.AppendLine(tableBlock.TableType.ToString());
                IndexBlock indexBlockHead = fs.ReadBlock<IndexBlock>(tableBlock.IndexBlockHeadPos);
                IndexBlock currentIndexBlock = indexBlockHead;
                IndexBlock indexBlock = fs.ReadBlock<IndexBlock>(currentIndexBlock.NextPos);

                SkipListNodeBlock currentHeadNode = fs.ReadBlock<SkipListNodeBlock>(indexBlock.SkipListHeadNodePos);
                int level = db.GetDBHeaderBlock().MaxLevelOfSkipList - 1;
                SkipListNodeBlock current = currentHeadNode;
                while (current != null)// 依次Print表的PK Index
                {
                    StringBuilder levelBuilder = new StringBuilder();
                    levelBuilder.AppendLine(string.Format("level {0}", level--));
                    string str = Print(current);
                    levelBuilder.AppendLine(str);
                    int count = 1;
                    while (current.RightPos != 0)//依次Print PK Index的Level
                    {
                        current = fs.ReadBlock<SkipListNodeBlock>(current.RightPos);
                        str = Print(current);
                        levelBuilder.AppendLine(str);
                        count++;
                    }

                    if (count > 2)
                    { builder.AppendLine(levelBuilder.ToString()); }

                    if (currentHeadNode.DownPos != 0)
                    { currentHeadNode = fs.ReadBlock<SkipListNodeBlock>(currentHeadNode.DownPos); }
                    else
                    { currentHeadNode = null; }
                    current = currentHeadNode;
                }

                currentTableBlock = tableBlock;
            }
        }

        /// <summary>
        /// 获取数据库的所有表和表里的所有数据对象。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static SharpFileDBInfo GetDBInfo(this FileDBContext db)
        {
            SharpFileDBInfo info = new SharpFileDBInfo();

            FileStream fs = db.GetFileStream();
            fs.ReadBlock<PageHeaderBlock>(0);
            fs.ReadBlock<DBHeaderBlock>(fs.Position);
            TableBlock tableHead = fs.ReadBlock<TableBlock>(fs.Position);
            TableBlock currentTableBlock = tableHead;
            while (currentTableBlock.NextPos != 0)
            {
                TableBlock tableBlock = fs.ReadBlock<TableBlock>(currentTableBlock.NextPos);
                TableInfo tableInfo = new TableInfo(tableBlock);
                info.Add(tableInfo);
                IndexBlock indexBlockHead = fs.ReadBlock<IndexBlock>(tableBlock.IndexBlockHeadPos);
                IndexBlock currentIndexBlock = indexBlockHead;
                bool primaryKey = true;
                while (currentIndexBlock.NextPos != 0)
                {
                    IndexBlock indexBlock = fs.ReadBlock<IndexBlock>(currentIndexBlock.NextPos);
                    tableInfo.Add(indexBlock);
                    if (primaryKey)
                    {
                        long currentHeadNodePos = indexBlock.SkipListHeadNodePos;
                        SkipListNodeBlock currentHeadNode = fs.ReadBlock<SkipListNodeBlock>(currentHeadNodePos);
                        currentHeadNode.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.DownObj);
                        while (currentHeadNode.DownObj != null)
                        {
                            currentHeadNode.DownObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.DownObj);
                            currentHeadNode = currentHeadNode.DownObj;
                        }
                        SkipListNodeBlock current = currentHeadNode;
                        current.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);
                        while (current.RightObj != null && current.RightObj.ThisPos != indexBlock.SkipListTailNodePos)
                        {
                            current.RightObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.Value);
                            Table item = current.RightObj.Value.GetObject<Table>(db);

                            tableInfo.Add(item);

                            current = current.RightObj;
                            current.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);
                        }
                        //while (current.RightObj.RightPos != 0)
                        //{
                        //    current.RightObj.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.Value);
                        //    Table item = current.RightObj.Value.GetObject<Table>(db);

                        //    tableInfo.Add(item);

                        //    current = current.RightObj;
                        //}

                        primaryKey = false;
                    }

                    currentIndexBlock = indexBlock;
                }

                currentTableBlock = tableBlock;
            }

            return info;
        }
    }
}
