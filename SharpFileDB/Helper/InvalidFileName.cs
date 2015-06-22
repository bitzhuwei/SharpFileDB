using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// Contains invalid chars in a file name.
    /// </summary>
    internal static class InvalidFileName
    {
        private static HashSet<char> invalidFileNameChars;

        public static HashSet<char> InvalidFileNameChars
        {
            get { return InvalidFileName.invalidFileNameChars; }
        }

        static InvalidFileName()
        {
            invalidFileNameChars = new HashSet<char>() { '\0', ' ', '.', '$', '/', '\\' };
            foreach (var c in Path.GetInvalidPathChars()) { invalidFileNameChars.Add(c); }
            foreach (var c in Path.GetInvalidFileNameChars()) { invalidFileNameChars.Add(c); }
        }

    }
}
