using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpFileDB.Zoo
{
    [Serializable]
    public class Hobby
    {
        public double SportHour { get;set; }

        public string Note { get; set; }

        /// <summary>
        /// 显示此对象的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("sport hour: {0}, note: {1}", SportHour, Note);
        }
    }
}
