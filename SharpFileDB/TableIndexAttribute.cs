using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 标记此属性应作为<see cref="Table"/>的一个索引。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class TableIndexAttribute : Attribute
    {

        /// <summary>
        /// 标记此属性应作为<see cref="Table"/>的一个索引。
        /// </summary>
        ///// <param name="unique">标识该索引是否唯一索引。即代表此字段的值在表中不能重复。比如说一个员工表，员工ID, 身份证肯定不会重复，不过唯一索引并不一定是作为主码。</param>
        ///// <param name="ascending">true表示该索引是升序排序，false表示是降序。</param>
        public TableIndexAttribute()//bool unique = true)//, bool ascending = true)
        {
            //this.Unique = unique;
            //this.Ascending = ascending;
        }


        ///// <summary>
        ///// 标识该索引是否唯一索引。即代表此字段的值在表中不能重复。比如说一个员工表，员工ID, 身份证肯定不会重复，不过唯一索引并不一定是作为主码。
        ///// </summary>
        //public bool Unique { get; set; }

        ///// <summary>
        ///// true表示该索引是升序排序，false表示是降序。
        ///// </summary>
        //public bool Ascending { get; set; }

        /// <summary>
        /// 显示此对象的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return string.Format("Unique: {0}, Ascending: {1}", this.Unique, this.Ascending);
            //return string.Format("Unique: {0}", this.Unique);
            return string.Format("{0}", this.GetType().FullName);
        }
    }
}
