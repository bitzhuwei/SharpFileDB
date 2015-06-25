using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Xml.Serialization;

namespace SharpFileDB.TestConsole
{
    public class DemoSingleFileDB
    {
        [Serializable]
        public class Cat
        {
            public override string ToString()
            {
                return string.Format("{0}: {1}", this.Id, this.Name);
            }

            public Image HeadPortrait { get; set; }

            public string Name { get; set; }

            public int Id { get; set; }
        }

        [Serializable]
        public class Fish 
        {
            public override string ToString()
            {
                return string.Format("{0}: {1}", this.Id, this.Weight);
            }

            public float Weight { get; set; }


            public int Id { get; set; }
        }

        /// <summary>
        /// 此Demo证明一个事实：formatter.Serialize(fs, msg);就是向fs写入一个msg，再调用formatter.Serialize(fs, cat);就是继续写入一个cat。fs是通过Position属性指定读写位置的。
        /// 就是说，FileStream可以被看做一个连续的字节数组byte[]，且此数组可以无限自动向后延伸。
        /// 但是SoapFormatter在反序列化过程中，fs.Seek()对其无效。也就是说，SoapFormatter无法用于随机反序列化。详情参见（http://www.cnblogs.com/bitzhuwei/p/how-to-serialize-multiple-objects-in-one-file-and-random-deserialize-any-object.html）
        /// </summary>
        internal static void TypicalScene()
        {
            //BinaryFormatter: 能够随机读写，适合做单文件数据库的序列化和反序列化工具。
            {
                IFormatter formatter = new BinaryFormatter();

                Cat cat = new Cat() { Id = 1, Name = "汤姆" };
                Cat cat2 = new Cat() { Id = 2, Name = "汤姆媳妇" };
                Fish fish = new Fish() { Id = 3, Weight = 1.5f };

                using (FileStream fs = new FileStream("singleFileDB.bin", FileMode.Create, FileAccess.ReadWrite))
                {
                    formatter.Serialize(fs, cat);
                    formatter.Serialize(fs, cat2);
                    formatter.Serialize(fs, fish);
                }

                object obj = null;

                using (FileStream fs = new FileStream("singleFileDB.bin", FileMode.Open, FileAccess.Read))
                {
                    obj = formatter.Deserialize(fs);
                    Console.WriteLine(string.Format("{0}/{1}", fs.Position, fs.Length));
                    long position = fs.Position;
                    obj = formatter.Deserialize(fs);
                    Console.WriteLine(string.Format("{0}/{1}", fs.Position, fs.Length));
                    fs.Position = position;
                    obj = formatter.Deserialize(fs);
                    Console.WriteLine(string.Format("{0}/{1}", fs.Position, fs.Length));
                    obj = formatter.Deserialize(fs);
                    Console.WriteLine(string.Format("{0}/{1}", fs.Position, fs.Length));
                }
            }
            //SoapFormatter: 每次反序列化时都会一次性读到文件末尾。Stream.Position对其随机读写没有意义。
            {
                IFormatter formatter = new SoapFormatter();
                Cat cat = new Cat() { Id = 1, Name = "汤姆" };//, HeadPortrait=Image.FromFile("CatHeadPortrait.png") };
                Cat cat2 = new Cat() { Id = 2, Name = "汤姆媳妇" };
                Fish fish = new Fish() { Id = 3, Weight = 1.5f };

                using (FileStream fs = new FileStream("singleFileDB.soap", FileMode.Create, FileAccess.ReadWrite))
                {
                    formatter.Serialize(fs, cat);
                    formatter.Serialize(fs, cat2);
                    formatter.Serialize(fs, fish);
                }

                object obj = null;

                using (FileStream fs = new FileStream("singleFileDB.soap", FileMode.Open, FileAccess.Read))
                {
                    Console.WriteLine(fs.Position == fs.Length);// false

                    obj = formatter.Deserialize(fs);
                    Console.WriteLine(fs.Position == fs.Length);// true

                    obj = formatter.Deserialize(fs);
                    Console.WriteLine(fs.Position == fs.Length);// true

                    obj = formatter.Deserialize(fs);
                    Console.WriteLine(fs.Position == fs.Length);// true
                }
            }

            // XmlSerializer：只能处理public类型；不能在同一文档中存储多个序列化的对象。
            {
                Cat cat = new Cat() { Id = 1, Name = "汤姆" };
                Cat cat2 = new Cat() { Id = 2, Name = "汤姆媳妇" };
                Fish fish = new Fish() { Id = 3, Weight = 1.5f };

                using (FileStream fs = new FileStream("singleFileDB.xml", FileMode.Create, FileAccess.ReadWrite))
                {
                    XmlSerializer formatter = new XmlSerializer(cat.GetType());//SharpFileDB.TestConsole.DemoSingleFileDB+Cat 因其保护级别而不可访问。只能处理公共类型。
                    formatter.Serialize(fs, cat);
                    formatter.Serialize(fs, cat2);
                    formatter = new XmlSerializer(fish.GetType());
                    formatter.Serialize(fs, fish);
                }

                object obj = null;

                using (FileStream fs = new FileStream("singleFileDB.xml", FileMode.Open, FileAccess.Read))
                {
                    while (true)
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(Cat));
                        obj = formatter.Deserialize(fs);//意外的 XML 声明。XML 声明必须是文档中的第一个节点并且在它之前不允许出现空白字符。 第 5 行，位置 9。
                        Console.WriteLine(string.Format("{0}/{1}", fs.Position, fs.Length));
                        //int randomePosition = random.Next(0, (int)fs.Length);
                        //fs.Seek(0, SeekOrigin.Begin);
                        obj = formatter.Deserialize(fs);
                        Console.WriteLine(string.Format("{0}/{1}", fs.Position, fs.Length));

                        formatter = new XmlSerializer(typeof(Fish));
                        obj = formatter.Deserialize(fs);
                        Console.WriteLine(string.Format("{0}/{1}", fs.Position, fs.Length));
                        obj = formatter.Deserialize(fs);
                        Console.WriteLine(string.Format("{0}/{1}", fs.Position, fs.Length));
                    }
                }
            }
        }

    }
}
