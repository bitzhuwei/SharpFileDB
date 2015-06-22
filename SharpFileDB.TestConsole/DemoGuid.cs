using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    class DemoGuid
    {
        public static void TypicalScene()
        {
            // what Guid.ToString() looks like:
            Guid id = Guid.NewGuid();
            var e = id.ToString();
            var E = id.ToString("");
            var n = id.ToString("N");
            var d = id.ToString("D");
            var b = id.ToString("B");
            var p = id.ToString("P");
            var x = id.ToString("X");
        }
    }
}
