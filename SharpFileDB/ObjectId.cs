using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// 用于生成唯一的<see cref="Table"/>编号。
    /// </summary>
    [Serializable]
    public sealed class ObjectId : ISerializable, IComparable<ObjectId>, IComparable
    {
        private string _string;

        private ObjectId()
        {
        }

        public ObjectId(string value)
            : this(DecodeHex(value))
        {
        }

        internal ObjectId(byte[] value)
        {
            Value = value;
        }

        public static ObjectId Empty
        {
            get { return new ObjectId("000000000000000000000000"); }
        }

        public byte[] Value { get; private set; }

        public static ObjectId NewId()
        {
            return new ObjectId { Value = ObjectIdGenerator.Generate() };
        }

        public static bool TryParse(string value, out ObjectId objectId)
        {
            objectId = Empty;
            if (value == null || value.Length != 24)
            {
                return false;
            }

            try
            {
                objectId = new ObjectId(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        static byte[] DecodeHex(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            var chars = value.ToCharArray();
            var numberChars = chars.Length;
            var bytes = new byte[numberChars / 2];

            for (var i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(new string(chars, i, 2), 16);
            }

            return bytes;
        }

        public override int GetHashCode()
        {
            return Value != null ? ToString().GetHashCode() : 0;
        }

        public override string ToString()
        {
            if (_string == null && Value != null)
            {
                _string = BitConverter.ToString(Value)
                  .Replace("-", string.Empty)
                  .ToLowerInvariant();
            }

            return _string;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ObjectId;
            return Equals(other);
        }

        public bool Equals(ObjectId other)
        {
            return other != null && ToString() == other.ToString();
        }

        public static implicit operator string(ObjectId objectId)
        {
            return objectId == null ? null : objectId.ToString();
        }

        public static implicit operator ObjectId(string value)
        {
            return new ObjectId(value);
        }

        public static bool operator ==(ObjectId left, ObjectId right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ObjectId left, ObjectId right)
        {
            return !(left == right);
        }

        #region ISerializable 成员

        const string strValue = "";
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string value = this.ToString();
            info.AddValue(strValue, value);
        }

        #endregion

        private ObjectId(SerializationInfo info, StreamingContext context)
        {
            string value = info.GetString(strValue);
            this.Value = DecodeHex(value);
        }


        #region IComparable<ObjectId> 成员

        public int CompareTo(ObjectId other)
        {
            if (other == null) { return 1; }

            string thisStr = this.ToString();
            string otherStr = other.ToString();
            int result = thisStr.CompareTo(otherStr);

            return result;
        }

        #endregion

        #region IComparable 成员

        public int CompareTo(object obj)
        {
            ObjectId other = obj as ObjectId;
            return CompareTo(other);
        }

        #endregion
    }

    internal static class ObjectIdGenerator
    {
        private static readonly DateTime Epoch =
          new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly object _innerLock = new object();
        private static int _counter;
        private static readonly byte[] _machineHash = GenerateHostHash();
        private static readonly byte[] _processId =
          BitConverter.GetBytes(GenerateProcessId());

        public static byte[] Generate()
        {
            var oid = new byte[12];
            var copyidx = 0;

            Array.Copy(BitConverter.GetBytes(GenerateTime()), 0, oid, copyidx, 4);
            copyidx += 4;

            Array.Copy(_machineHash, 0, oid, copyidx, 3);
            copyidx += 3;

            Array.Copy(_processId, 0, oid, copyidx, 2);
            copyidx += 2;

            Array.Copy(BitConverter.GetBytes(GenerateCounter()), 0, oid, copyidx, 3);

            return oid;
        }

        private static int GenerateTime()
        {
            var now = DateTime.UtcNow;
            var nowtime = new DateTime(Epoch.Year, Epoch.Month, Epoch.Day,
              now.Hour, now.Minute, now.Second, now.Millisecond);
            var diff = nowtime - Epoch;
            return Convert.ToInt32(Math.Floor(diff.TotalMilliseconds));
        }

        private static byte[] GenerateHostHash()
        {
            using (var md5 = MD5.Create())
            {
                var host = Dns.GetHostName();
                return md5.ComputeHash(Encoding.Default.GetBytes(host));
            }
        }

        private static int GenerateProcessId()
        {
            var process = Process.GetCurrentProcess();
            return process.Id;
        }

        private static int GenerateCounter()
        {
            lock (_innerLock)
            {
                return _counter++;
            }
        }
    }
}
