using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDatabase.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestJsonDatabase();
            TestXmlDatabase();

            Console.ReadKey();
        }

        //private static void TestJsonDatabase()
        //{
        //    JsonDatabase db = new JsonDatabase(@"C:\tmp");
        //    db.OutputIndent = true;

        //    Cat origin = new Cat() { Name = "Garfield", Legs = 4 };
        //    db.Save<Cat>(origin);

        //    db.Save<Cat>(origin.Id, origin);
        //    db.Delete<Cat>(origin.Id);
        //}

        private static void TestXmlDatabase()
        {
            XmlDatabase db = new XmlDatabase(@"C:\tmp");
            db.OutputIndent = true;

            Cat origin = new Cat() { Name = "Garfield", Legs = 4 };
            db.Save<Cat>(origin);

            db.Save<Cat>(origin.Id, origin);

            IEnumerable<Cat> founds = db.FindAll<Cat>();

            Cat found = db.FindOneById<Cat>(origin.Id);
            

            db.Delete<Cat>(origin.Id);
        }
    }
}
