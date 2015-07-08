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
        
        /// <summary>
        /// 使用FileStream和BinaryFormatter做单文件数据库的核心工作流。
        /// </summary>
        /// <param name="fullname"></param>
        public static void TypicalScene(string fullname)
        {
            // 初始化。
            BinaryFormatter formatter = new BinaryFormatter();

            // 打开数据库文件。
            FileStream fs = new FileStream(fullname, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);

            // 把对象写入数据库。
            long position = 0;// 指定位置。
            fs.Seek(position, SeekOrigin.Begin);
            Object obj = new Object();// 此处可以是任意具有[Serializable]特性的类型。
            formatter.Serialize(fs, obj);// 把对象序列化并写入文件。

            fs.Flush();

            // 从数据库文件读取对象。
            fs.Seek(position, SeekOrigin.Begin);// 指定位置。
            Object deserialized = formatter.Deserialize(fs);// 从文件得到反序列化的对象。

            // 关闭文件流，退出数据库。
            fs.Close();
            fs.Dispose();
        }
    }
}
