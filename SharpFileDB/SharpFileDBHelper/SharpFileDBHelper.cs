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
                bool pk = true;
                while (currentIndexBlock.NextPos != 0)
                {
                    IndexBlock indexBlock = fs.ReadBlock<IndexBlock>(currentIndexBlock.NextPos);
                    tableInfo.Add(indexBlock);
                    if (pk)
                    {
                        long currentHeadNodePos = indexBlock.SkipListHeadNodePos;
                        SkipListNodeBlock currentHeadNode = null;
                        while (currentHeadNodePos != 0)
                        {
                            currentHeadNode = fs.ReadBlock<SkipListNodeBlock>(currentHeadNodePos);
                            currentHeadNodePos = currentHeadNode.DownPos;
                        }
                        SkipListNodeBlock currentSkipListNode = currentHeadNode;
                        while (currentSkipListNode.RightPos != 0)
                        {
                            SkipListNodeBlock node = fs.ReadBlock<SkipListNodeBlock>(currentSkipListNode.RightPos);
                            DataBlock dataBlock = fs.ReadBlock<DataBlock>(node.ValuePos);

                            byte[] valueBytes = new byte[dataBlock.ObjectLength];

                            int index = 0;// index == dataBlock.ObjectLength - 1时，dataBlock.NextDataBlockPos也就正好应该等于0了。
                            for (int i = 0; i < dataBlock.Data.Length; i++)
                            { valueBytes[index++] = dataBlock.Data[i]; }
                            while (dataBlock.NextPos != 0)
                            {
                                dataBlock = fs.ReadBlock<DataBlock>(dataBlock.NextPos);
                                for (int i = 0; i < dataBlock.Data.Length; i++)
                                { valueBytes[index++] = dataBlock.Data[i]; }
                            }

                            Table item = valueBytes.ToObject<Table>();

                            tableInfo.Add(item);

                            currentSkipListNode = node;
                        }

                        pk = false;
                    }

                    currentIndexBlock = indexBlock;
                }

                currentTableBlock = tableBlock;
            }

            return info;
        }
    }
}
