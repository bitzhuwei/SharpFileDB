using SharpFileDB;
using System;

namespace SharpFileDB.TestConsole
{
    /// <summary>
    /// demo file object
    /// </summary>
    [Serializable]
    public class Cat : FileObject
    {
        public string Name { get; set; }
        public int Legs { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Name: {1}, Legs: {2}", base.ToString(), Name, Legs);
        }
    }
}
