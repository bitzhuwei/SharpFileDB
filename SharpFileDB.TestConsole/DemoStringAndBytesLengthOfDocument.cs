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
    class DemoStringAndBytesLengthOfDocument
    {
        public static void TypcialScene()
        {
            Pet pet1 = new Pet() { Price = 1 };
            pet1.Id = DocumentId.NewId();
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, pet1);
                long length = ms.Length;
                //byte[] bytes = new byte[length];
                ms.Position = 0;
                //ms.Read(bytes, 0, bytes.Length);
                object obj = formatter.Deserialize(ms);
            }
        }


    }

    [Serializable]
    class Pet : Document
    {
        public Pet() { }

        public string Name { get; set; }
        public int Price { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("p", this.Price);
            info.AddValue("n", this.Name);
        }

        protected Pet(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Price = info.GetInt32("p");
            this.Name = info.GetString("n");
        }
    }
}
