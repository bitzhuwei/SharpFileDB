using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    public static class IOExceptionHelper
    {
        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;

        /// <summary>
        /// 等待指定的时间；如果已经等待过了就抛出异常。
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="timer"></param>
        public static void WaitIfLocked(this IOException ex, int timer)
        {
            var errorCode = Marshal.GetHRForException(ex) & ((1 << 16) - 1);
            if (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION)
            {
                if (timer > 0)
                {
                    Thread.Sleep(timer);
                }
            }
            else
            {
                throw ex;
            }
        }
    }
}
