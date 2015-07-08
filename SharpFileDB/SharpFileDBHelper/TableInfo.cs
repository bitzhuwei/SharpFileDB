using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpFileDB.SharpFileDBHelper
{
    public class TableInfo
    {
        private Blocks.TableBlock tableBlock;

        public List<IndexInfo> indexInfoList = new List<IndexInfo>();
        public List<Table> recordList = new List<Table>();

        public TableInfo(Blocks.TableBlock tableBlock)
        {
            this.tableBlock = tableBlock;
            this.TableType = tableBlock.TableType;
        }

        public Type TableType { get; set; }

        public void Add(Blocks.IndexBlock indexBlock)
        {
            IndexInfo indexInfo = new IndexInfo(indexBlock);

            this.indexInfoList.Add(indexInfo);
        }

        public override string ToString()
        {
            return string.Format("{0}", this.TableType);
            //return base.ToString();
        }

        public void Add(Table item)
        {
            this.recordList.Add(item);
        }
    }
}
