using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    internal static class FileObjectIdGenerator
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly object _innerLock = new object();
        private static int _counter;
        private static readonly byte[] _machineHash = GenerateHostHash();
        private static readonly byte[] _processId =
          BitConverter.GetBytes(GenerateProcessId());

        public static byte[] Generate()
        {
            var oid = new byte[12];
            var copyidx = 0;

            Array.Copy(BitConverter.GetBytes(GenerateTime()), 0, oid, copyidx, 4);
            copyidx += 4;

            Array.Copy(_machineHash, 0, oid, copyidx, 3);
            copyidx += 3;

            Array.Copy(_processId, 0, oid, copyidx, 2);
            copyidx += 2;

            Array.Copy(BitConverter.GetBytes(GenerateCounter()), 0, oid, copyidx, 3);

            return oid;
        }

        private static int GenerateTime()
        {
            var now = DateTime.UtcNow;
            var nowtime = new DateTime(Epoch.Year, Epoch.Month, Epoch.Day,
              now.Hour, now.Minute, now.Second, now.Millisecond);
            var diff = nowtime - Epoch;
            return Convert.ToInt32(Math.Floor(diff.TotalMilliseconds));
        }

        private static byte[] GenerateHostHash()
        {
            using (var md5 = MD5.Create())
            {
                var host = Dns.GetHostName();
                return md5.ComputeHash(Encoding.Default.GetBytes(host));
            }
        }

        private static int GenerateProcessId()
        {
            var process = Process.GetCurrentProcess();
            return process.Id;
        }

        private static int GenerateCounter()
        {
            lock (_innerLock)
            {
                return _counter++;
            }
        }
    }
}
