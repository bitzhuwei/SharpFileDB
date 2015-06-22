using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    public static class ImageHelper
    {
        /// <summary>
        /// Convert Image to Byte[]
        /// <para>把Image转换为byte[]</para>
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image image)
        {
            byte[] buffer = null;

            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);

                buffer = new byte[ms.Length];
                //Image.Save() changed the ms.Position. So we need to Seek it to the beginning.
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
            }

            return buffer;
        }

        /// <summary>
        /// Convert Byte[] to a picture and Store it in file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = ByteArrayHelper.ToImage(buffer);
            ImageFormat format = image.RawFormat;

            string extension = GetExtesion(image);
            file += "." + extension;
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }

        public static string GetExtesion(this Image image)
        {
            string extension = string.Empty;

            if (image != null)
            {
                ImageFormat format = image.RawFormat;
                if (format.Equals(ImageFormat.Bmp))
                {
                    extension = "bmp";
                }
                else if (format.Equals(ImageFormat.Emf))
                {
                    extension = "emf";
                }
                else if (format.Equals(ImageFormat.Exif))
                {
                    extension = "exif";
                }
                //else if (format.Equals(ImageFormat.flashPIX))
                //{
                //extension = "flashPIX";
                //}
                else if (format.Equals(ImageFormat.Gif))
                {
                    extension = "gif";
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    extension = "icon";
                }
                else if (format.Equals(ImageFormat.Jpeg))
                {
                    extension = "jpeg";
                }
                else if (format.Equals(ImageFormat.MemoryBmp))
                {
                    extension = "bmp";
                }
                //else if (format.Equals(ImageFormat.photoCD))
                //{
                //extension = "photoCD";
                //}
                else if (format.Equals(ImageFormat.Png))
                {
                    extension = "png";
                }
                else if (format.Equals(ImageFormat.Tiff))
                {
                    extension = "tiff";
                }
                else if (format.Equals(ImageFormat.Wmf))
                {
                    extension = "wmf";
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return extension;
        }
    }

}
