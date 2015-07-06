using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    class DemoHowBinaryFormatterWorks
    {
        public static void TypicalScene(string fullname)
        {
            FileStream fs = new FileStream(fullname, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            BinaryFormatter formatter = new BinaryFormatter();

            long position = 0;// 指定位置。
            fs.Seek(position, SeekOrigin.Begin);
            Object obj = new Object();// 此处可以是任意具有[Serializable]特性的类型。
            formatter.Serialize(fs, obj);// 把对象序列化并写入文件。

            fs.Flush();

            fs.Seek(position, SeekOrigin.Begin);// 指定位置。
            Object deserialized = formatter.Deserialize(fs);// 从文件得到反序列化的对象。

            // do something with deserialized ..
        }
    }
}
