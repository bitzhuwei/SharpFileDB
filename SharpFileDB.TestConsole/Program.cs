using SharpFileDB.Zoo;
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
        static Random random = new Random();

        static void Main(string[] args)
        {

            //DemoStringAndBytesLengthOfDocument.TypcialScene();
            //DemoTypeSerialization.TypicalScene();
            //DemoDocumentMeta.TypicalScene();
            //DemoSingleFileDB.TypicalScene2();
            //DemoSingleFileDB.TypicalScene();
            //DemoMaxFilesInDirectory.TypicalScene();
            //DemoGuid.TypicalScene();
            //DemoTcpMsg.TypicalScene();

            //IPersistence ipersistence = new DefaultPersistence();
            string fullname = Path.Combine(Environment.CurrentDirectory, "TestDatabase", "test.db");
            //string fullname = @"D:\360云盘\文档\TestDatabase\test.db";
            //string fullname = @"C:\Users\DELL\Documents\百度云同步盘\SharpFileDB\TestDatabase\test.db";

            // common cases to use SharpFileDB.
            FileDBContext db = new FileDBContext(fullname);

            Cat cat = new Cat();
            string name = "kitty白 " + random.Next();
            cat.KittyName = name;
            cat.Price = random.Next(1, 10);
            cat.FavoriteFood = new Food() { FoodName = "小黄鱼", };
            cat.HeadPortrait = Image.FromFile(@"CatHeadPortrait.png");
            cat.OwnHobby = new Hobby() { Note = "this takes some time", SportHour = 2.5 };
            Lion lion = new Lion();
            lion.KittyName = "狮子 " + random.Next();
            lion.Price = random.Next(1, 10);
            lion.FavoriteFood = new Food() { FoodName = "小黄鱼", };
            lion.HeadPortrait = Image.FromFile(@"CatHeadPortrait.png");
            lion.OwnHobby = new Hobby() { Note = "this takes some time", SportHour = 2.5 };
            db.Insert(cat);
            db.Insert(lion);

            //IList<Cat> cats = db.Find<Cat>(x => x.KittyName == name);
            System.Linq.Expressions.Expression<Func<Cat, bool>> pre =
                (x => (x.KittyName == name && x.Price < 10) || (x.KittyName == "kitty2" && x.Price > 10));
            IList<Cat> cats = db.Find<Cat>(pre);

            cats = db.FindAll<Cat>();

            cat.KittyName = "小白 " + random.Next();
            db.Update(cat);

            //foreach (var item in cats)
            //{
            //    db.Delete(item);
            //}
            db.Delete(cat);
            //if (cats.Count > 0)
            //{
            //    Cat first = cats[0];
            //    db.Delete(first);
            //}
        }
    }
}
