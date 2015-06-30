using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    class DemoTypeSerialization
    {
        public static void TypicalScene()
        {
            Cat c = new Cat();
            Type type = c.GetType();
            string str = type.FullName;
            Type t = Type.GetType(str);
            Guid guid = type.GUID;
            Type t2 = Type.GetTypeFromCLSID(guid);
        }
    }
}
