using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 用<see cref="IFormatter"/>实现<see cref="IPersistence"/>。
    /// <para>Implement <see cref="IPersistence"/> using <see cref="IFormatter"/>.</para>
    /// </summary>
    public class DefaultPersistence : IPersistence
    {
        private System.Runtime.Serialization.IFormatter formatter;

        public DefaultPersistence(PersistenceFormat format = PersistenceFormat.Soap)
        {
            switch (format)
            {
                case PersistenceFormat.Soap:
                    this.formatter = new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
                    this.Extension = "soap";
                    break;
                case PersistenceFormat.Binary:
                    this.formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    this.Extension = "bin";
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        #region IPersistence 成员

        private string extension;
        public string Extension
        {
            get { return "bin"; }
            private set { this.extension = value; }
        }

        public string Serialize(FileObject item)
        {
            if (item == null)
            {
                return null;
            }

            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, item);
                ms.Position = 0;
                bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);
            }
            string str = Convert.ToBase64String(bytes);
            return str;
        }

        public TFileObject Deserialize<TFileObject>(string serializedFileObject) where TFileObject : FileObject
        {
            TFileObject fileObjct = null;

            if (!string.IsNullOrEmpty(serializedFileObject))
            {
                byte[] bytes = Convert.FromBase64String(serializedFileObject);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    ms.Position = 0;
                    object obj = formatter.Deserialize(ms);
                    fileObjct = obj as TFileObject;
                }
            }

            return fileObjct;
        }

        #endregion
      
    }

    public enum PersistenceFormat
    {
        Soap,
        Binary,
    }
}
