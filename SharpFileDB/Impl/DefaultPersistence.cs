﻿using System;
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

        public DefaultPersistence()
        {
            this.formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            this.Extension = "bin";
        }

        #region IPersistence 成员

        private string extension;
        public string Extension
        {
            get { return this.extension; }
            private set { this.extension = value; }
        }

        public void Serialize(Document item, string fullname)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (string.IsNullOrEmpty(fullname))
            {
                throw new ArgumentNullException("fullname");
            }

            using (FileStream s = new FileStream(fullname, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(s, item);
            }
        }

        public TDocument Deserialize<TDocument>(string fullname) where TDocument : Document
        {
            if(string.IsNullOrEmpty(fullname))
            {
                throw new ArgumentNullException("fullname");
            }

            TDocument fileObject = null;

            using (FileStream s = new FileStream(fullname, FileMode.Open, FileAccess.Read))
            {
                object obj = formatter.Deserialize(s);
                fileObject = obj as TDocument;
            }

            return fileObject;
        }

        #endregion

    }


}
