using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpFileDB.Blocks;
using SharpFileDB.Utilities;

namespace SharpFileDB.SharpFileDBHelper
{
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

        public static string Print(this FileDBContext db)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(db.Fullname);
            FileStream fs = db.fileStream;
            PageHeaderBlock firstPage = fs.ReadBlock<PageHeaderBlock>(0);
            DBHeaderBlock dbHeader = fs.ReadBlock<DBHeaderBlock>(fs.Position);

            PrintSkipLists(db, builder, fs);

            builder.AppendLine();
            builder.AppendLine("Page Info.:");
            for (long i = 0; i < db.fileStream.Length; i += Consts.pageSize)
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
                builder.Append(string.Format(" {0} ->", pageInfo.ThisPos));
                tablePos = pageInfo.NextPagePos;
            }
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("index pages:");
            long indexPos = dbHeader.FirstIndexPagePos;
            while (indexPos != 0)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(indexPos);
                builder.Append(string.Format(" {0} ->", pageInfo.ThisPos));
                indexPos = pageInfo.NextPagePos;
            }
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("skip list node pages:");
            long skiplistnodePos = dbHeader.FirstSkipListNodePagePos;
            while (skiplistnodePos != 0)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(skiplistnodePos);
                builder.Append(string.Format(" {0} ->", pageInfo.ThisPos));
                skiplistnodePos = pageInfo.NextPagePos;
            }
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("data block pages:");
            long dataBlockPos = dbHeader.FirstDataPagePos;
            while (dataBlockPos != 0)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(dataBlockPos);
                builder.Append(string.Format(" {0} ->", pageInfo.ThisPos));
                dataBlockPos = pageInfo.NextPagePos;
            }
            builder.AppendLine();

            builder.AppendLine();
            builder.AppendLine("empty pages:");
            long emptyPos = dbHeader.FirstEmptyPagePos;
            while (emptyPos != 0)
            {
                PageHeaderBlock pageInfo = fs.ReadBlock<PageHeaderBlock>(emptyPos);
                builder.Append(string.Format(" {0} ->", pageInfo.ThisPos));
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
                int level = db.headerBlock.MaxLevelOfSkipList - 1;
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

        public static SharpFileDBInfo GetDBInfo(this FileDBContext db)
        {
            SharpFileDBInfo info = new SharpFileDBInfo();

            FileStream fs = db.fileStream;
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
                        currentHeadNode.TryLoadRightDownObj(fs, LoadOptions.DownObj);
                        while (currentHeadNode.DownObj != null)
                        {
                            currentHeadNode.DownObj.TryLoadRightDownObj(fs, LoadOptions.DownObj);
                            currentHeadNode = currentHeadNode.DownObj;
                        }
                        SkipListNodeBlock current = currentHeadNode;
                        current.TryLoadRightDownObj(fs, LoadOptions.RightObj);
                        while (current.RightObj.RightPos != 0)
                        {
                            current.RightObj.TryLoadRightDownObj(fs, LoadOptions.Value);
                            Table item = current.RightObj.Value.GetObject<Table>(db);

                            tableInfo.Add(item);

                            current = current.RightObj;
                        }
                        //long currentHeadNodePos = indexBlock.SkipListHeadNodePos;
                        //SkipListNodeBlock currentHeadNode = null;
                        //while (currentHeadNodePos != 0)
                        //{
                        //    currentHeadNode = fs.ReadBlock<SkipListNodeBlock>(currentHeadNodePos);
                        //    currentHeadNodePos = currentHeadNode.DownPos;
                        //}
                        //SkipListNodeBlock currentSkipListNode = currentHeadNode;
                        //while (currentSkipListNode.RightPos != 0)
                        //{
                        //    SkipListNodeBlock node = fs.ReadBlock<SkipListNodeBlock>(currentSkipListNode.RightPos);
                        //    DataBlock dataBlock = fs.ReadBlock<DataBlock>(node.ValuePos);

                        //    byte[] valueBytes = new byte[dataBlock.ObjectLength];

                        //    int index = 0;// index == dataBlock.ObjectLength - 1时，dataBlock.NextDataBlockPos也就正好应该等于0了。
                        //    for (int i = 0; i < dataBlock.Data.Length; i++)
                        //    { valueBytes[index++] = dataBlock.Data[i]; }
                        //    while (dataBlock.NextPos != 0)
                        //    {
                        //        dataBlock = fs.ReadBlock<DataBlock>(dataBlock.NextPos);
                        //        for (int i = 0; i < dataBlock.Data.Length; i++)
                        //        { valueBytes[index++] = dataBlock.Data[i]; }
                        //    }

                        //    Table item = valueBytes.ToObject<Table>();

                        //    tableInfo.Add(item);

                        //    currentSkipListNode = node;
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
