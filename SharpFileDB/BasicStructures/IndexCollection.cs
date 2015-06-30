using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.BasicStructures
{
    /// <summary>
    /// 数据库所有的Index集合。
    /// <para>string为Index的名称，对应table-type的字段/属性名。</para>
    /// <para>IndexNode为skip list的第一个结点。</para>
    /// </summary>
    public class IndexCollection : PersistableDictionary<string, IndexNode>
    {
    }
}