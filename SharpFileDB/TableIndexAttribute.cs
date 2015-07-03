using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 标记此字段/属性应作为<see cref="Table"/>的一个索引。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class TableIndexAttribute : Attribute
    {
        public TableIndexAttribute()
        {
        }

    }
}
