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

        /// <summary>
        /// 测试：用BinaryFormatter读写数据库。
        /// </summary>
        internal static void TypicalScene2()
        {
            // 理论上的单文件数据库的长度最大为long.MaxValue(9223372036854775807)个字节，即8589934591GB = 8388607TB = 8191PB = 7EB
            const string strHowSingleFileDBWorks = "HowSingleFileDBWorks.db";

            // 首先，创建数据库文件。
            using (FileStream fs = new FileStream(strHowSingleFileDBWorks, FileMode.Create, FileAccess.Write))
            { }

            // 然后，在App中创建了一些对象。
            Cat cat = new Cat() { Id = 1, Name = "汤姆" };
            Cat cat2 = new Cat() { Id = 2, Name = "汤姆的媳妇" };
            Fish fish = new Fish() { Id = 3, Weight = 1.5f };


            // 然后，用某种序列化方式将其写入数据库。
            IFormatter formatter = new BinaryFormatter();

            // 写入cat
            long catLength = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bytes;
                formatter.Serialize(ms, cat);
                ms.Position = 0;
                bytes = new byte[ms.Length];
                catLength = ms.Length;// 在实际数据库中，catLength由文件字节管理器进行读写
                ms.Read(bytes, 0, bytes.Length);
                using (FileStream fs = new FileStream(strHowSingleFileDBWorks, FileMode.Open, FileAccess.Write))
                {
                    fs.Position = 0;// 在实际数据库中，需要指定对象要存储到的位置
                    fs.Write(bytes, 0, bytes.Length);//注意，若bytes.Length超过int.MaxValue，这里就需要特殊处理了。
                }
            }

            // 写入cat2
            long cat2Length = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bytes;
                formatter.Serialize(ms, cat2);
                ms.Position = 0;
                bytes = new byte[ms.Length];
                cat2Length = ms.Length;// 在实际数据库中，cat2Length由文件字节管理器进行读写
                ms.Read(bytes, 0, bytes.Length);
                using (FileStream fs = new FileStream(strHowSingleFileDBWorks, FileMode.Open, FileAccess.Write))
                {
                    fs.Position = catLength;// 在实际数据库中，需要指定对象要存储到的位置
                    fs.Write(bytes, 0, bytes.Length);//注意，若bytes.Length超过int.MaxValue，这里就需要特殊处理了。
                }
            }

            // 写入fish
            long fishLength = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bytes;
                formatter.Serialize(ms, fish);
                ms.Position = 0;
                bytes = new byte[ms.Length];
                fishLength = ms.Length;// 在实际数据库中，fishLength由文件字节管理器进行读写
                ms.Read(bytes, 0, bytes.Length);
                using (FileStream fs = new FileStream(strHowSingleFileDBWorks, FileMode.Open, FileAccess.Write))
                {
                    fs.Position = catLength + cat2Length;// 在实际数据库中，需要指定对象要存储到的位置
                    fs.Write(bytes, 0, bytes.Length);//注意，若bytes.Length超过int.MaxValue，这里就需要特殊处理了。
                }
            }

            //查询cat2
            using (FileStream fs = new FileStream(strHowSingleFileDBWorks, FileMode.Open, FileAccess.Read))
            {
                fs.Position = catLength;// 在实际数据库中，需要指定对象存储到的位置
                object obj = formatter.Deserialize(fs);
                Console.WriteLine(obj);// {2: 汤姆的媳妇}
            }

            //删除cat2
            // 在实际数据库中，这由文件字节管理器进行控制，只需标记cat2所在的空间为没有占用即可。实际操作是修改几个skip list指针。

            //新增cat3
            Cat cat3 = new Cat() { Id = 4, Name = "喵" };
            long cat3Length = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] bytes;
                formatter.Serialize(ms, cat3);
                ms.Position = 0;
                bytes = new byte[ms.Length];
                cat3Length = ms.Length;// 在实际数据库中，cat3Length由文件字节管理器进行读写
                ms.Read(bytes, 0, bytes.Length);
                using (FileStream fs = new FileStream(strHowSingleFileDBWorks, FileMode.Open, FileAccess.Write))
                {
                    fs.Position = catLength;// 在实际数据库中，需要指定对象要存储到的位置，这里由文件字节管理器为其找到可插入的空闲空间。
                    fs.Write(bytes, 0, bytes.Length);//注意，若bytes.Length超过int.MaxValue，这里就需要特殊处理了。
                }
            }

            //查询cat cat3 fish
            using (FileStream fs = new FileStream(strHowSingleFileDBWorks, FileMode.Open, FileAccess.Read))
            {
                object obj = null;
                // cat
                fs.Position = 0;// 在实际数据库中，需要指定对象存储到的位置
                obj = formatter.Deserialize(fs);
                Console.WriteLine(obj);// {1: 汤姆}
                
                // cat3
                fs.Position = catLength;// 在实际数据库中，需要指定对象存储到的位置

                obj = formatter.Deserialize(fs);
                Console.WriteLine(obj);// {4: 喵}
                
                // fish
                fs.Position = catLength + cat2Length;// 在实际数据库中，需要指定对象存储到的位置

                obj = formatter.Deserialize(fs);
                Console.WriteLine(obj);// {3: 1.5}
            }

        }

    }
}
