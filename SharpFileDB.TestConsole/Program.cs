using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DemoGuid.TypicalScene();
            DemoBinaryFormater.TypicalScene();

            {
                List<Type> ipersistenceList = new List<Type>();
                Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(IPersistence));
                foreach (var item in assembly.ExportedTypes)
                {
                    if (typeof(IPersistence).IsAssignableFrom(item))
                    {
                        if (item.IsClass)
                        {
                            ipersistenceList.Add(item);
                        }
                    }
                }

                foreach (var item in ipersistenceList)
                {
                    object obj = Activator.CreateInstance(item);
                    IPersistence ipersistence = obj as IPersistence;
                    //string dbDirectory = Path.Combine(Environment.CurrentDirectory, "TestDatabase");
                    string dbDirectory = @"D:\360云盘\文档\TestDatabase";

                    // common cases to use SharpFileDB.
                    FileDBContext db = new FileDBContext(dbDirectory, ipersistence);

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
}
