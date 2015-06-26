using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    internal class FreeSpaceManager
    {
        internal long invalidFreeSpacePosition = -1;

        //SkipList<long, long> freeBytesDict = new SkipList<long, long>(maxLevel, 0.5f, Comparer<long>.Default);
        //List<long> freeSpacePositionList = new List<long>();
        //List<long> freeSpaceLengthList = new List<long>();
        //List<long> freeSpaceNextList = new List<long>();
        internal List<FreeSpaceNode> freeSpaceNodeList = new List<FreeSpaceNode>();
        //internal List<long> nodePositionList = new List<long>();
        internal long firstNodePosition;

        public long AllocateFreeSpace(long length, Transaction transaction)
        {
            long position = invalidFreeSpacePosition;

            for (int i = 1; i < freeSpaceNodeList.Count; i++)
            {
                FreeSpaceNode item = freeSpaceNodeList[i];
                FreeSpaceNode previous = freeSpaceNodeList[i-1];

                //if (item.Length >= length)
                //{
                //    position = item.Position;
                //    item.Position += length;
                //    item.Length -= length;

                //    if (item.Length > length)
                //    {
                //        if (!transaction.ContainsKey(item))
                //        {
                //            transaction.Add(item, previous.NextNodePosition);
                //        }
                //    }
                //    else
                //    {
                //        previous.NextNodePosition = item.NextNodePosition;
                //        if (!transaction.ContainsKey(previous))
                //        {
                //            transaction.Add(previous, );
                //        }
                //        freeSpaceNodeList.RemoveAt(i);
                //        nodePositionList.RemoveAt(i);
                //    }

                //    break;
                //}
            }

            return position;
        }
    }

    //internal class FreeSpaceBlock
    //{
    //    internal FreeSpaceBlock(FreeSpaceNode node, long requestedLength)
    //    {
    //        this.node = node;
    //        this.requestedLength = requestedLength;
    //    }

    //    public override string ToString()
    //    {
    //        return string.Format("{0} - {1}", node, requestedLength);
    //    }

    //    public FreeSpaceNode node { get; set; }

    //    public long requestedLength { get; set; }
    //}
}
