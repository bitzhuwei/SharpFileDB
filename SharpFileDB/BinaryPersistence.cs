using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 用BinaryFormatter实现<see cref="IPersistence"/>。
    /// <para>Implement <see cref="IPersistence"/> using BinaryFormatter.</para>
    /// </summary>
    public class BinaryPersistence : IPersistence
    {

        #region IPersistence 成员

        public string Extension
        {
            get { return "bin"; }
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
                BinaryFormatter formatter = new BinaryFormatter();
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
                    BinaryFormatter formatter = new BinaryFormatter();
                    object obj = formatter.Deserialize(ms);
                    fileObjct = obj as TFileObject;
                }
            }

            return fileObjct;
        }

        #endregion
        //#region IPersistence 成员

        //public string Extension
        //{
        //    get { return "bin"; }
        //}

        //public void Serialize(FileObject item, string fullname)
        //{
        //    if (item == null) { return; }

        //    byte[] bytes;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        BinaryFormatter formatter = new BinaryFormatter();
        //        formatter.Serialize(ms, item);
        //        ms.Position = 0;
        //        bytes = new byte[ms.Length];
        //        ms.Read(bytes, 0, bytes.Length);
        //    }

        //    File.WriteAllBytes(fullname, bytes);
        //}

        //public TFileObject Deserialize<TFileObject>(string fullname) where TFileObject : FileObject
        //{
        //    TFileObject fileObjct = null;

        //    byte[] bytes = File.ReadAllBytes(fullname);

        //    using (MemoryStream ms = new MemoryStream(bytes))
        //    {
        //        ms.Position = 0;
        //        BinaryFormatter formatter = new BinaryFormatter();
        //        object obj = formatter.Deserialize(ms);
        //        fileObjct = obj as TFileObject;
        //    }

        //    return fileObjct;
        //}

        //#endregion
    }
}
