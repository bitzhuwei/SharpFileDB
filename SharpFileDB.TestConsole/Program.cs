using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
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
            {
                // common cases to use SharpFileDB.
                FileDBContext db = new FileDBContext();

                Cat cat = new Cat();
                cat.Name = "xiao xiao bai";
                db.Create(cat);

                Predicate<Cat> pre = new Predicate<Cat>(x => x.Name == "xiao xiao bai");
                IList<Cat> cats = db.Retrieve(pre);

                cat.Name = "xiao bai";
                db.Update(cat);

                db.Delete(cat);
            }
        }
    }
}
