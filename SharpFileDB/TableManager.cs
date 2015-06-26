using NGenerics.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    class TableManager
    {
        const int defaultMaxLevel = 32;
        const float defaultProbability = 0.5f;

        internal SkipList<Type, long> tableDict;

        public TableManager(int maxLevel = defaultMaxLevel, float probability = defaultProbability)
        {
            this.tableDict = new SkipList<Type, long>(maxLevel, probability, new TypeComparer());
        }

        class TypeComparer : IComparer<Type>
        {

            #region IComparer<Type> 成员

            public int Compare(Type x, Type y)
            {
                if (x == null)
                {
                    if (y == null)
                    { return 0; }
                    else
                    { return -1; }
                }
                else
                {
                    if (y == null)
                    { return 1; }
                    else
                    {
                        int xCode = x.GetHashCode();
                        int yCode = y.GetHashCode();
                        int result = xCode = yCode;
                        return result;
                    }
                }
            }

            #endregion
        }

    }
}
