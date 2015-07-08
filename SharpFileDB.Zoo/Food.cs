using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SharpFileDB.Zoo
{
    [Serializable]
    public class Food : ISerializable
    {

        /// <summary>
        /// 显示此对象的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("food: {0}", this.FoodName);
        }
        public string FoodName { get; set; }

        public Food() { }

        #region ISerializable 成员

        const string strFoodName = "F";

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(strFoodName, this.FoodName);
        }

        #endregion

        protected Food(SerializationInfo info, StreamingContext context)
        {
            this.FoodName = info.GetString(strFoodName);
        }
    }
}
