using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SharpFileDB.TestConsole
{
    [Serializable]
    class Food : ISerializable
    {

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
