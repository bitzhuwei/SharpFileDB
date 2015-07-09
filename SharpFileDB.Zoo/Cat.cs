using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpFileDB.Zoo
{
    [Serializable]
    public class Cat : Table
    {

        /// <summary>
        /// 显示此对象的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return string.Format("{0}: ￥{1}, {2}, {3}", KittyName, Price, FavoriteFood, OwnHobby);
            return string.Format("{0}: ￥{1}", KittyName, Price);
        }
        public string KittyName { get; set; }

        public int Price { get; set; }

        //public System.Drawing.Image HeadPortrait { get; set; }

        //public Food FavoriteFood { get; set; }

        //public Hobby OwnHobby { get; set; }

        public Cat() { }

        const string strKittyName = "N";
        const string strPrice = "P";
        //const string strHeadPortrait = "T";
        //const string strFood = "F";
        //const string strHobby = "H";

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(strKittyName, this.KittyName);
            info.AddValue(strPrice, this.Price);
            //info.AddValue(strHeadPortrait, this.HeadPortrait);
            //info.AddValue(strFood, this.FavoriteFood);
            //info.AddValue(strHobby, this.OwnHobby);
        }

        protected Cat(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            this.KittyName = info.GetString(strKittyName);
            this.Price = info.GetInt32(strPrice);
            //this.HeadPortrait = (System.Drawing.Image)info.GetValue(strHeadPortrait, typeof(System.Drawing.Image));
            //this.FavoriteFood = (Food)info.GetValue(strFood, typeof(Food));
            //this.OwnHobby = (Hobby)info.GetValue(strHobby, typeof(Hobby));
        }

    }
}
