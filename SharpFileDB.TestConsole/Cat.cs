using SharpFileDB;
using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SharpFileDB.TestConsole
{
    /// <summary>
    /// demo file object
    /// <para></para>
    /// More information please visit: (http://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.iserializable.aspx) and (https://msdn.microsoft.com/zh-cn/library/system.runtime.serialization.iserializable.getobjectdata.aspx).
    /// </summary>
    [Serializable]
    public class Cat : FileObject, ISerializable
    {
        /// <summary>
        /// Used for serialization.
        /// <para>序列化需要此构造函数。</para>
        /// </summary>
        public Cat() { }

        public string Name { get; set; }
        public int Legs { get; set; }

        public Image HeadPortrait { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Name: {1}, Legs: {2}", base.ToString(), Name, Legs);
        }

        const string strName = "Name";
        const string strLegs = "Legs";
        const string strHeadPortraitString = "headPortraitString";

        #region ISerializable 成员

        /// <summary>
        /// This method will be invoked automatically when IFormatter.Serialize() is called.
        /// <para>当使用IFormatter.Serialize()时会自动调用此方法。</para>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        //[SecurityPermission(SecurityAction.LinkDemand,
            //Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue(strName, this.Name);

            info.AddValue(strLegs, this.Legs);

            if (this.HeadPortrait != null)
            {
                byte[] bytes = ImageHelper.ToBytes(this.HeadPortrait);
                string str = Convert.ToBase64String(bytes);
                info.AddValue(strHeadPortraitString, str);
            }
            else
            {
                info.AddValue(strHeadPortraitString, string.Empty);
            }
        }

        #endregion

        /// <summary>
        /// This constructor will be invoked automatically when IFormatter.Deserialize() is called.
        /// <para>当使用IFormatter.Deserialize()时会自动调用此构造函数。</para>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Cat(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            this.Name = (string)info.GetValue(strName, typeof(string));

            this.Legs = (int)info.GetValue(strLegs, typeof(int));

            object obj = info.GetValue(strHeadPortraitString, typeof(string));
            if (obj != null)
            {
                string str = obj as string;
                if (str != string.Empty)
                {
                    byte[] bytes = Convert.FromBase64String(str);
                    Image image = ByteArrayHelper.ToImage(bytes);
                    this.HeadPortrait = image;
                }
            }
        }
    }
}
