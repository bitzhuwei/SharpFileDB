using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.TestConsole
{
    [Serializable]
    class KittyCat : Cat
    {
        public KittyCat() { }

        public override string ToString()
        {
            return string.Format("{0},{1}", AgeInMonth, base.ToString());
        }

        public int AgeInMonth { get; set; }

        const string strAgeInMonth = "AgeInMonth";
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(strAgeInMonth, AgeInMonth);
        }

        protected KittyCat(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.AgeInMonth = (int)info.GetValue(strAgeInMonth, typeof(int));
        }
    }
}
