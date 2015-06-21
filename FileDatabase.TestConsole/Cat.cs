using System;

namespace FileDatabase.TestConsole
{
    [Serializable]
    public class Cat
    {
        public Cat()
        {
            Id = ObjectId.NewObjectId().ToString();
        }

        public Cat(string id)
        {
            Id = id;
        }

        public string Name { get; set; }
        public int Legs { get; set; }

        public string Id { get; set; }

        public override string ToString()
        {
            return string.Format("DocumentId={0}, Name={1}, Legs={2}", Id, Name, Legs);
        }
    }
}
