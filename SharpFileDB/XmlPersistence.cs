using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SharpFileDB
{
    /// <summary>
    /// 用XmlSerializer实现<see cref="IPersistence"/>。
    /// <para>Implement <see cref="IPersistence"/> using XmlSerializer.</para>
    /// </summary>
    public class XmlPersistence : IPersistence
    {
        #region IPersistence 成员

        public string Extension
        {
            get { return "xml"; }
        }

        public string Serialize(FileObject item)
        {
            using (StringWriterWithEncoding sw = new StringWriterWithEncoding(Encoding.UTF8))
            {
                XmlSerializer serializer = new XmlSerializer(item.GetType());
                serializer.Serialize(sw, item);
                string serializedString = sw.ToString();

                return serializedString;
            }
        }

        public TFileObject Deserialize<TFileObject>(string serializedFileObject)
            where TFileObject : FileObject
        {
            if (string.IsNullOrEmpty(serializedFileObject))
                throw new ArgumentNullException("data");

            using (StringReader sr = new StringReader(serializedFileObject))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TFileObject));
                object deserializedObj = serializer.Deserialize(sr);
                TFileObject fileObject = deserializedObj as TFileObject;
                return fileObject;
            }
        }

        #endregion
    }
}
