using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDatabase
{
    public class ObjectId
    {
        private string _string;

        public ObjectId()
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

        public static ObjectId NewObjectId()
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
    }
}
