using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.VisualDesigner
{
    class TableDesigner
    {
        /// <summary>
        /// 包括了命名空间的全名。
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        /// 不包括Table.Id在内的属性。
        /// </summary>
        List<PropertyDesigner> PropertyDesignerList = new List<PropertyDesigner>();

        public string ToCSharpCode()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("using System;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("using System.Linq;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("using SharpFileDB;");
            builder.AppendLine("");

            int tabSpace = 0;
            string _namespace = this.Fullname.GetNameSpace();
            if (!string.IsNullOrEmpty(_namespace))
            {
                builder.AppendLine("namespace " + _namespace);
                builder.AppendLine("{");
            }
            else
            { tabSpace += 4; }

            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("public partial class " + this.Fullname.GetClassName() + " : Table");
            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("{");// public partial class Xxx : Table {

            tabSpace += 4;
            foreach (var item in this.PropertyDesignerList)
            {
                item.ToCSharpCode(builder, tabSpace);
            }

            tabSpace -= 4;

            builder.PrintTabSpace(tabSpace);
            if (!string.IsNullOrEmpty(_namespace))
            { builder.AppendLine("}"); }// }// end of class
            else
            { tabSpace -= 4; }

            return builder.ToString();
        }

     
    }

    static class TableDesignerHelper
    {

        public static void PrintTabSpace(this StringBuilder builder, int tabSpace)
        {
            for (int i = 0; i < tabSpace; i++)
            { builder.Append(' '); }
        }

        public static string GetNameSpace(this string fullname)
        {
            int index = fullname.LastIndexOf('.');
            if (index >= 0)
            { return fullname.Substring(0, index); }
            else
            { return string.Empty; }
        }

        public static string GetClassName(this string fullname)
        {
            int index = fullname.LastIndexOf('.');
            if (index >= 0)
            { return fullname.Substring(index); }
            else
            { return fullname; }
        }
    }
}
