using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    class DemoDocumentMeta
    {
        /// <summary>
        /// 用DocumentMeta占用162字节，用long占用58字节，用BinaryWriter.Write()占用8字节。
        /// 所以，放在<see cref="Document"/>前方的用于表示此<see cref="Document"/>字节数的metadata用BinaryWriter.Write()来读写。
        /// </summary>
        public static void TypicalScene()
        {
            {
                for (long meta = 1L; meta < 100L; meta++)
                {
                    byte[] serialized = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (BinaryWriter bw = new BinaryWriter(ms))
                        {
                            bw.Write(meta);
                            serialized = new byte[ms.Length];// 8
                            ms.Position = 0;
                            ms.Read(serialized, 0, serialized.Length);
                        }
                    }
                    using (MemoryStream ms = new MemoryStream(serialized))
                    {
                        using (BinaryReader br = new BinaryReader(ms))
                        {
                            long result = br.ReadInt64();
                            Console.WriteLine(result == meta);
                        }
                    }
                }
            }
            {
                for (long meta = 1L; meta < 100L; meta++)
                {
                    byte[] serialized = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        formatter.Serialize(ms, meta);
                        ms.Position = 0;
                        serialized = new byte[ms.Length];
                        ms.Read(serialized, 0, serialized.Length);// 58
                    }
                    using (MemoryStream ms = new MemoryStream(serialized))
                    {
                        object obj = formatter.Deserialize(ms);
                        Console.WriteLine((long)obj == meta);
                    }
                }
            }
            //long defaultMetaLength = 0;
            //{
            //    DocumentMeta meta = new DocumentMeta() { Length = 0L };
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        formatter.Serialize(ms, meta);
            //        defaultMetaLength = ms.Length;// 162
            //    }
            //}

            //for (long i = 0; i < 100L; i++)
            //{
            //    TestLength(i, defaultMetaLength);
            //}

            //Random random = new Random();

            //int count = 0;
            //for (long i = 0; i < long.MaxValue && count < 1001; i += long.MaxValue / random.Next(1, 1001))
            //{
            //    TestLength(i, defaultMetaLength);
            //    count++;
            //}
        }

        //static void TestLength(long value, long defaultMetaLength)
        //{
        //    DocumentMeta meta = new DocumentMeta() { Length = value };
        //    byte[] bytes;

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        formatter.Serialize(ms, meta);
        //        ms.Position = 0;
        //        if (ms.Length != defaultMetaLength)
        //        { throw new Exception("design error: DocumentMeta is not fixed size!"); }
        //        bytes = new byte[ms.Length];
        //        ms.Read(bytes, 0, bytes.Length);
        //    }

        //    using (MemoryStream ms=new MemoryStream(bytes))
        //    {
        //        object obj = formatter.Deserialize(ms);
        //        Console.WriteLine(obj);
        //    }
        //}

        static IFormatter formatter = new BinaryFormatter();
    }
}
