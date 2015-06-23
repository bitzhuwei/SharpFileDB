using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    class DemoTcpMsg
    {
        /// <summary>
        /// 这个Demo证明了序列化对继承也是完美解决的。
        /// </summary>
        public static void TypicalScene()
        {
            List<System.Runtime.Serialization.IFormatter> formatterList = 
                new List<System.Runtime.Serialization.IFormatter>() 
            { new SoapFormatter(), new BinaryFormatter(), };

            foreach (var formatter in formatterList)
            {
                Cat cat = new KittyCat() { Legs = 3, Name = "hello kitty小猫咪", AgeInMonth = 3, HeadPortrait = Image.FromFile(@"CatHeadPortrait.png") };
                TcpMsg msg = new TcpMsg() { IPAddress = "127.0.0.1", Content = cat };

                byte[] serializedBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, msg);
                    ms.Position = 0;
                    serializedBytes = new byte[ms.Length];
                    ms.Read(serializedBytes, 0, serializedBytes.Length);
                }

                TcpMsg gotMsg = null;

                byte[] gotBytes = serializedBytes; // Transform through network.

                using (MemoryStream ms = new MemoryStream(gotBytes))
                {
                    ms.Position = 0;
                    object obj = formatter.Deserialize(ms);
                    gotMsg = obj as TcpMsg;
                }
            }

        }
    }
}
