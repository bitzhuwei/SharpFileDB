using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 实现索引的接口。SharpFileDB只有一个索引。
    /// <para>Interface for index. There's only one index in SharpFileDB</para>
    /// </summary>
    public interface IIndex
    {
        /// <summary>
        /// 获取索引值。
        /// <para>Gets index value.</para>
        /// </summary>
        /// <returns></returns>
        string GetIndex();
    }
}
