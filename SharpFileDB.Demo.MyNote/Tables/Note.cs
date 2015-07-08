using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Demo.MyNote.Tables
{
    /// <summary>
    /// 表类型Note。
    /// The table-type Note.
    /// </summary>
    [Serializable]
    public class Note : Table
    {

        /// <summary>
        /// 显示此对象的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}", this.Title);
        }

        public string Title { get; set; }
        public string Content { get; set; }
        public Note() { }

        string strTitle = "Title";
        string strContent = "Content";

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(strTitle, this.Title);
            info.AddValue(strContent, this.Content);
        }

        protected Note(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Title = (string)info.GetValue(strTitle, typeof(string));
            this.Content = (string)info.GetValue(strContent, typeof(string));
        }

    }
}
