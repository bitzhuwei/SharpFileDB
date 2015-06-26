using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SharpFileDB
{
    public class TransactionItem
    {
        public long Position { get; set; }

        public byte[] serializedBytes { get; set; }

        public void Write(Stream stream, IFormatter formatter)
        {
            stream.Seek(this.Position, SeekOrigin.Begin);
            long length = (long)serializedBytes.Length;
            if (serializedBytes.LongLength == length)
            {
                stream.Write(serializedBytes, 0, serializedBytes.Length);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
