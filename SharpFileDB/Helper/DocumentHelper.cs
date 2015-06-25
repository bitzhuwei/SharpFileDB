using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    static class DocumentHelper
    {
        /// <summary>
        /// 生成文件名，此文件将用于存储此<see cref="Document"/>的内容。
        /// Generate file name that will contain this instance's data of <see cref="Document"/>.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="extension">文件扩展名。<para>File's extension name.</para></param>
        /// <returns></returns>
        internal static string GenerateFileName(this Document doc, string extension)
        {
            string id = doc.Id.ToString();

            string name = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", id, extension);

            return name;
        }
    }
}
