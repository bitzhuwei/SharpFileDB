using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileDatabase
{
    class FileDatabaseException : Exception
    {
        private string p;
        private Exception ex;

        public FileDatabaseException(string p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }

        public FileDatabaseException(string p, Exception ex)
        {
            // TODO: Complete member initialization
            this.p = p;
            this.ex = ex;
        }
    }
}
