using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpFileDB.VisualDesigner
{
    public class PropertyDesigner
    {

        public override string ToString()
        {
            return string.Format("{0} {1} {{ get; set; }} //({2})", this.PropertyType, this.PropertyName, this.IndexType);
        }

        public IndexTypes IndexType { get; set; }

        public string PropertyType { get; set; }

        public string PropertyName { get; set; }

        public string XmlNote { get; set; }

        public void ToCSharpCode(StringBuilder builder, int tabSpace)
        {
            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("/// <summary>");

            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("/// " + XmlNote);

            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("/// </summary>");

            switch (this.IndexType)
            {
                case IndexTypes.NoIndex:
                    break;
                case IndexTypes.UniqueIndex:
                    builder.PrintTabSpace(tabSpace);
                    builder.AppendLine("[TableIndex]");
                    break;
                //case IndexTypes.NonUniqueIndex:
                //    builder.PrintTabSpace(tabSpace);
                //    builder.AppendLine("[TableIndex]");
                //    break;
                default:
                    throw new NotImplementedException();
            }

            builder.PrintTabSpace(tabSpace);
            builder.AppendLine("public " + this.PropertyType + " " + this.PropertyName+" { get; set; }");

            builder.AppendLine();
        }
    }

    /// <summary>
    /// 索引类型。
    /// </summary>
    public enum IndexTypes
    {

        /// <summary>
        /// 不是索引。
        /// </summary>
        NoIndex,
        
        /// <summary>
        /// 唯一索引。
        /// </summary>
        UniqueIndex,

        ///// <summary>
        ///// 非唯一索引。
        ///// </summary>
        //NonUniqueIndex,
    }
}
