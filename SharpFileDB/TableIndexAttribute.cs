using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 标记此属性应作为<see cref="Table"/>的一个索引。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class TableIndexAttribute : Attribute
    {
        internal TableIndexAttribute()
        {
        }

    }
}
