using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 
    /// </summary>
    public class Transaction : List<IPointToNextInFile>
    {
        IFormatter formatter = new BinaryFormatter();

        public void Commit(FileStream fs)
        {
            //long fsPosition = fs.Position;

            //List<byte[]> newBytesList = new List<byte[]>();
            //List<long> positionList = new List<long>();

            using (FileStream rollBackFile = new FileStream(fs.Name + "rollback", FileMode.CreateNew, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(rollBackFile))
                {
                    foreach (var item in this)
                    {
                        // 要写入数据库的字节们.
                        //byte[] newBytes = item.Key.Serialize();
                        byte[] newBytes = item.Serialize();
                        //newBytesList.Add(newBytes);
                        //positionList.Add(item.Value);
                        // 即将被覆盖的旧数据。
                        byte[] oldBytes = new byte[newBytes.Length];
                        //fs.Seek(item.Value, SeekOrigin.Begin);
                        fs.Seek(item.SerializedPositionInFile, SeekOrigin.Begin);
                        fs.Read(oldBytes, 0, oldBytes.Length);
                        // 临时备份旧数据。
                        //formatter.Serialize(rollBackFile, item.PositionInFile);
                        bw.Write(item.SerializedPositionInFile);
                        rollBackFile.Write(oldBytes, 0, oldBytes.Length);
                    }
                }
            }

            //for (int i = 0; i < positionList.Count; i++)
            //{
            //    long position = positionList[i];
            //    byte[] newBytes = newBytesList[i];
            //    fs.Seek(position, SeekOrigin.Begin);
            //    fs.Write(newBytes, 0, newBytes.Length);
            //}
            foreach (var item in this)
            {
                //fs.Seek(item.Value, SeekOrigin.Begin);
                fs.Seek(item.SerializedPositionInFile, SeekOrigin.Begin);
                formatter.Serialize(fs, item);
            }

            //fs.Seek(fsPosition, SeekOrigin.Begin);

            File.Delete(fs.Name + "rollback");
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }
    }
    //public class Transaction : Dictionary<object, long>
    //{
    //    IFormatter formatter = new BinaryFormatter();

    //    /// <summary>
    //    /// Write something at specified position.
    //    /// </summary>
    //    /// <param name="fs">database file's stream.</param>
    //    public void Commit(FileStream fs)
    //    {
    //        long fsPosition = fs.Position;

    //        List<byte[]> newBytesList = new List<byte[]>();
    //        List<long> positionList = new List<long>();

    //        using (FileStream rollBackFile = new FileStream(fs.Name + "rollback", FileMode.CreateNew, FileAccess.Write))
    //        {
    //            foreach (var item in this)
    //            {
    //                // 要写入数据库的字节们.
    //                byte[] newBytes = item.Key.Serialize();
    //                newBytesList.Add(newBytes);
    //                positionList.Add(item.Value);
    //                // 即将被覆盖的旧数据。
    //                byte[] oldBytes = new byte[newBytes.Length];
    //                fs.Seek(item.Value, SeekOrigin.Begin);
    //                fs.Read(oldBytes, 0, oldBytes.Length);
    //                // 临时备份旧数据。
    //                formatter.Serialize(rollBackFile, item.Value);
    //                rollBackFile.Write(oldBytes, 0, oldBytes.Length);
    //            }
    //        }

    //        for (int i = 0; i < positionList.Count; i++)
    //        {
    //            long position = positionList[i];
    //            byte[] newBytes = newBytesList[i];
    //            fs.Seek(position, SeekOrigin.Begin);
    //            fs.Write(newBytes, 0, newBytes.Length);
    //        }
    //        //foreach (var item in this)
    //        //{
    //        //    fs.Seek(item.Value, SeekOrigin.Begin);
    //        //    formatter.Serialize(fs, item.Key);
    //        //}

    //        fs.Seek(fsPosition, SeekOrigin.Begin);

    //        File.Delete(fs.Name + "rollback");
    //    }

    //    public void RollBack()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
