using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.VisualDesigner
{
    public class TableDesigner
    {

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }

        /// <summary>
        /// 表类型的类型名（不包含命名空间）。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表类型的Xml注释。
        /// </summary>
        public string XmlNote { get; set; }

        /// <summary>
        /// 不包括Table.Id在内的属性。
        /// </summary>
        public List<PropertyDesigner> PropertyDesignerList = new List<PropertyDesigner>();

        public string ToCSharpCode(string _namespace)
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
            //string _namespace = this.Name.GetNameSpace();
            if (!string.IsNullOrEmpty(_namespace))
            {
                builder.AppendLine("namespace " + _namespace);
                builder.AppendLine("{");
                tabSpace += 4;
            }

            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("/// <summary>");
            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("/// " + this.XmlNote);
            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("/// </summary>");
            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("[Serializable]");
            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("public partial class " + this.Name + " : Table");
            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("{");// public partial class Xxx : Table {

            tabSpace += 4;

            foreach (var item in this.PropertyDesignerList)
            {
                item.ToCSharpCode(builder, tabSpace);
            }

            tabSpace -= 4;

            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("}");// public partial class Xxx : Table {

            if (!string.IsNullOrEmpty(_namespace))
            {
                tabSpace -= 4;
                builder.PrintTabSpace(tabSpace);
                builder.AppendLine("}");
            }// }// end of namespace

            return builder.ToString();
        }


    }

    static class TableDesignerHelper
    {

        public static void PrintTabSpace(this StringBuilder builder, int tabSpace)
        {
            for (int i = 0; i < tabSpace; i++)
            { builder.Append(" "); }
        }

        //public static string GetNameSpace(this string fullname)
        //{
        //    int index = fullname.LastIndexOf('.');
        //    if (index >= 0)
        //    { return fullname.Substring(0, index); }
        //    else
        //    { return string.Empty; }
        //}

        //public static string GetClassName(this string fullname)
        //{
        //    int index = fullname.LastIndexOf('.');
        //    if (index >= 0)
        //    { return fullname.Substring(index); }
        //    else
        //    { return fullname; }
        //}
    }
}
