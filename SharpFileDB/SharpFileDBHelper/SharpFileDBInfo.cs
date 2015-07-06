using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpFileDB.SharpFileDBHelper
{
    public class SharpFileDBInfo
    {
        public List<TableInfo> tableList = new List<TableInfo>();

        public void Add(TableInfo tableInfo)
        {
            this.tableList.Add(tableInfo);
        }
    }
}
