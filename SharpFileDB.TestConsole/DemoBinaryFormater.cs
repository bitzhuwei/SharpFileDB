using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    class DemoBinaryFormater
    {
        public static void TypicalScene()
        {
            Cat cat = new Cat() { Legs = 3, Name = "hello kitty小猫咪" };

            byte[] serializedBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, cat);
                ms.Position = 0;
                serializedBytes = new byte[ms.Length];
                ms.Read(serializedBytes, 0, serializedBytes.Length);
            }
            string str = Convert.ToBase64String(serializedBytes); //System.Text.Encoding.Default.GetString(serializedBytes);

            Cat fileObjct = null;

            if (!string.IsNullOrEmpty(str))
            {
                byte[] bytes = Convert.FromBase64String(str); // System.Text.Encoding.Default.GetBytes(str);
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    ms.Position = 0;
                    BinaryFormatter formatter = new BinaryFormatter();
                    object obj = formatter.Deserialize(ms);
                    fileObjct = obj as Cat;
                }
            }

        }
    }
}
