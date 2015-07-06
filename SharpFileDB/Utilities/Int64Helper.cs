using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    internal static class Int64Helper
    {
        /// <summary>
        /// 或取给定位置所在的页的位置。
        /// </summary>
        /// <param name="blockPos"></param>
        /// <returns></returns>
        internal static long PagePos(this long blockPos)
        {
            if (blockPos < 0)
            { throw new Exception("block position must be no less than 0."); }

            long pagePos = blockPos - blockPos % Consts.pageSize;

            return pagePos;
        }

    }
}
