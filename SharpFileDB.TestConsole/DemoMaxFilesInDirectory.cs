using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    class DemoMaxFilesInDirectory
    {
        public static void TypicalScene()
        {
            string log = Path.Combine(Environment.CurrentDirectory, "maxFile.log");
            using (StreamWriter sw = new StreamWriter(log, false))
            {
                sw.WriteLine(DateTime.Now);

                System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                string path = Path.Combine(Environment.CurrentDirectory, "maxFile");
                Directory.CreateDirectory(path);
                using (FileStream s = new FileStream(path + "tmp", FileMode.Create, FileAccess.Write))
                {
                    formatter.Serialize(s, string.Empty);
                }
                Directory.Delete(path, true);
                return;

                Directory.CreateDirectory(path);
                
                double averageSpan = 0;
                double minAverageSpan = double.MaxValue;
                double maxAverageSpan = 0;

                Console.WriteLine("start to test performance of serailizing files into large folder...");
                sw.WriteLine("start to test performance of serailizing files into large folder...");
                for (int i = 0; i < int.MaxValue; i++)
                {
                    string fullname = Path.Combine(path, i.ToString());

                    var startTime = DateTime.Now.Ticks;

                    using (FileStream s = new FileStream(fullname, FileMode.Create, FileAccess.Write))
                    {
                        formatter.Serialize(s, string.Empty);
                    }

                    var endTime = DateTime.Now.Ticks;

                    var passed = endTime - startTime;
                    averageSpan = averageSpan * ((double)i / (double)(i + 1)) + (double)passed / (double)(i + 1);
                    if (averageSpan < minAverageSpan) { minAverageSpan = averageSpan; }
                    if (averageSpan > maxAverageSpan) { maxAverageSpan = averageSpan; }

                    if (i % 50000 == 49999)
                    {
                        Console.WriteLine("created [{0}] files, average cost: [{1}], this cost: [{2}]", i + 1, averageSpan, passed);
                        sw.WriteLine("created [{0}] files, average cost: [{1}], this cost: [{2}]", i + 1, averageSpan, passed);
                        sw.Flush();
                    }
                }

                Console.WriteLine("min cost: {0}, max cost: {1}", minAverageSpan, maxAverageSpan);
                sw.WriteLine("min cost: {0}, max cost: {1}", minAverageSpan, maxAverageSpan);
                Console.WriteLine("done");
                sw.WriteLine("done");

                sw.WriteLine(DateTime.Now);
            }
        }
    }
}
