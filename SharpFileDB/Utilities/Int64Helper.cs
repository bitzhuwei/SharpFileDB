using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// <see cref="Int64"/>类型的辅助类。
    /// </summary>
    public static class Int64Helper
    {
        /// <summary>
        /// 或取给定位置所在的页的位置。
        /// </summary>
        /// <param name="blockPos"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long PagePos(this long blockPos)
        {
            if (blockPos < 0)
            { throw new Exception("block position must be no less than 0."); }

            long pagePos = blockPos - blockPos % Consts.pageSize;

            return pagePos;
        }

    }
}
