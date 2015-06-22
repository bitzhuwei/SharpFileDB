using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    public static class ByteArrayHelper
    {
        /// <summary>
        /// Convert byte[] to Image
        /// 把byte[]转换为Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image ToImage(this byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }

    }
}
