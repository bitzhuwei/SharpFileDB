using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    /// <summary>
    /// Represents a TCP message between server and client.
    /// <para>TCP服务器和客户端收发的消息。</para>
    /// </summary>
    [Serializable]
    class TcpMsg : ISerializable
    {
        public override string ToString()
        {
            return string.Format("{0}", this.IPAddress);
            //return base.ToString();
        }
        public string IPAddress { get; set; }

        public Cat Content { get; set; }

        public TcpMsg() { }

        const string strIPAddress = "IPAddress";
        const string strContent = "Content";

        #region ISerializable 成员

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strIPAddress, this.IPAddress);
            info.AddValue(strContent, this.Content, typeof(Cat));
        }

        #endregion

        protected TcpMsg(SerializationInfo info, StreamingContext context)
        {
            this.IPAddress = (string)info.GetValue(strIPAddress, typeof(string));
            this.Content = (Cat)info.GetValue(strContent, typeof(Cat));
        }
    }
}
