using SharpFileDB.Blocks;
using SharpFileDB.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
//using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// <see cref="Table"/>类型的辅助类。
    /// </summary>
    public static class TableHelper
    {

        /// <summary>
        /// 把Table的一条记录转换为字节数组。这个字节数组应该保存到Data页。
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ToBytes(this Table table)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                Consts.formatter.Serialize(ms, table);
                if (ms.Length > (long)int.MaxValue)// RULE: 一条记录序列化后最长不能超过int.MaxValue个字节。
                { throw new Exception(string.Format("Toooo long is the [{0}]", table)); }
                result = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(result, 0, result.Length);
            }

            return result;
        }


        /// <summary>
        /// 把指定的数据库记录转换为<see cref="DataBlock"/>[]形式。
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DataBlock[] ToDataBlocks(this Table record)
        {
            byte[] bytes = record.ToBytes();

            // 准备data blocks。
            int dataBlockCount = (bytes.Length - 1) / Consts.maxDataBytes + 1;
            if (dataBlockCount <= 0)
            { throw new Exception(string.Format("no data block for [{0}]", record)); }
            DataBlock[] dataBlocks = new DataBlock[dataBlockCount];
            // 准备好最后一个data block。
            DataBlock lastDataBlock = new DataBlock() { ObjectLength = bytes.Length, };
            int lastLength = bytes.Length % Consts.maxDataBytes;
            if (lastLength == 0) { lastLength = Consts.maxDataBytes; }
            lastDataBlock.Data = new byte[lastLength];
            for (int i = bytes.Length - lastLength, j = 0; i < bytes.Length; i++, j++)
            { lastDataBlock.Data[j] = bytes[i]; }
            dataBlocks[dataBlockCount - 1] = lastDataBlock;
            // 准备其它data blocks。
            for (int i = dataBlockCount - 1 - 1; i >= 0; i--)
            {
                DataBlock block = new DataBlock() { ObjectLength = bytes.Length, };
                block.NextObj = dataBlocks[i + 1];
                block.Data = new byte[Consts.maxDataBytes];
                for (int p = i * Consts.maxDataBytes, q = 0; q < Consts.maxDataBytes; p++, q++)
                { block.Data[q] = bytes[p]; }
                dataBlocks[i] = block;
            }

            // dataBlocks[0] -> [1] -> [2] -> ... -> [dataBlockCount - 1] -> null
            return dataBlocks;
        }
    }
}
