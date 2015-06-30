using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.BasicStructures
{
    /// <summary>
    /// 数据库所有的Table集合。
    /// </summary>
    public class TableCollection : PersistableDictionary<Type, TableNode>
    {
    }
}
