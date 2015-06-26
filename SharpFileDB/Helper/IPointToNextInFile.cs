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
    internal static class IPointToNextInFileHelper
    {
        static IFormatter formatter = new BinaryFormatter();

        /// <summary>
        /// 获取数据库文件中的下一结点。
        /// <para>Gets next node from database file.</para>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal static IPointToNextInFile GetNext(this IPointToNextInFile node, FileStream stream)
        {
            long startPosition = node.NextSerializedPositionInFile;
            stream.Seek(startPosition, SeekOrigin.Begin);
            object obj = formatter.Deserialize(stream);// result.NextPositionInFile should be deserialized in formatter.Deserialize(stream);.
            long currentPosition = stream.Position;
            IPointToNextInFile result = (IPointToNextInFile)obj;
            result.SerializedPositionInFile = startPosition;
            result.SerializedLengthInFile = currentPosition - startPosition;
            return result;
        }
    }
}
