using System;
using System.Collections.Generic;
using System.Drawing;
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
            DemoDocumentMeta.TypicalScene();
            return;
            //DemoSingleFileDB.TypicalScene2();
            //DemoSingleFileDB.TypicalScene();
            //DemoMaxFilesInDirectory.TypicalScene();
            //DemoGuid.TypicalScene();
            //DemoTcpMsg.TypicalScene();

            {
                List<IPersistence> persistenceList = new List<IPersistence>()
                {
                    new DefaultPersistence(DefaultPersistence.PersistenceFormat.Soap),
                    new DefaultPersistence(DefaultPersistence.PersistenceFormat.Binary),
                };
                foreach (var ipersistence in persistenceList)
                {
                    //string dbDirectory = Path.Combine(Environment.CurrentDirectory, "TestDatabase");
                    //string dbDirectory = @"D:\360云盘\文档\TestDatabase";
                    string dbDirectory = @"C:\Users\DELL\Documents\百度云同步盘\SharpFileDB\TestDatabase";

                    // common cases to use SharpFileDB.
                    FileDBContext db = new FileDBContext(dbDirectory, ipersistence);

                    Cat cat = new Cat();
                    cat.Name = "xiao xiao bai";
                    cat.Legs = 4;
                    cat.HeadPortrait = Image.FromFile(@"CatHeadPortrait.png");
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
