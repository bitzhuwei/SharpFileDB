using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    internal static class DataBlockHelper
    {
        /// <summary>
        /// 以此块为第一个数据块读取一个对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataBlock"></param>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        internal static T GetObject<T>(this DataBlock dataBlock, FileStream fileStream)
        {
            byte[] bytes = new byte[dataBlock.ObjectLength];

            int index = 0;// index == dataBlock.ObjectLength - 1时，dataBlock.NextDataBlockPos也就正好应该等于0了。
            for (int i = 0; i < dataBlock.Data.Length; i++)
            { bytes[index++] = dataBlock.Data[i]; }
            while (dataBlock.NextPos != 0)
            {
                dataBlock = fileStream.ReadBlock<DataBlock>(dataBlock.NextPos);
                for (int i = 0; i < dataBlock.Data.Length; i++)
                { bytes[index++] = dataBlock.Data[i]; }
            }

            T obj = bytes.ToObject<T>();

            return obj;
        }

        /// <summary>
        /// 用这些数据块读取一个对象。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataBlocks"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static T GetObject<T>(this DataBlock[] dataBlocks, FileDBContext db)
        {
            byte[] bytes = new byte[dataBlocks[0].ObjectLength];
            int index = 0;// index == dataBlock.ObjectLength - 1时，dataBlock.NextDataBlockPos也就正好应该等于0了。
            foreach (DataBlock dataBlock in dataBlocks)
            {
                for (int i = 0; i < dataBlock.Data.Length; i++)
                {
                    bytes[index++] = dataBlock.Data[i];
                }
            }
            T obj = bytes.ToObject<T>();

            return obj;
        }

    }
}
