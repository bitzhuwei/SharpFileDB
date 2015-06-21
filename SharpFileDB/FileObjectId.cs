using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// Unique id for <see cref="FileObject"/>.
    /// </summary>
    public class FileObjectId
    {
        private string _string;

        public FileObjectId()
        {
        }

        public FileObjectId(string value)
            : this(DecodeHex(value))
        {
        }

        internal FileObjectId(byte[] value)
        {
            Value = value;
        }

        public static FileObjectId Empty
        {
            get { return new FileObjectId("000000000000000000000000"); }
        }

        public byte[] Value { get; private set; }

        public static FileObjectId NewObjectId()
        {
            return new FileObjectId { Value = FileObjectIdGenerator.Generate() };
        }

        public static bool TryParse(string value, out FileObjectId objectId)
        {
            objectId = Empty;
            if (value == null || value.Length != 24)
            {
                return false;
            }

            try
            {
                objectId = new FileObjectId(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        protected static byte[] DecodeHex(string value)
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
            var other = obj as FileObjectId;
            return Equals(other);
        }

        public bool Equals(FileObjectId other)
        {
            return other != null && ToString() == other.ToString();
        }

        public static implicit operator string(FileObjectId objectId)
        {
            return objectId == null ? null : objectId.ToString();
        }

        public static implicit operator FileObjectId(string value)
        {
            return new FileObjectId(value);
        }

        public static bool operator ==(FileObjectId left, FileObjectId right)
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

        public static bool operator !=(FileObjectId left, FileObjectId right)
        {
            return !(left == right);
        }
    }
}
