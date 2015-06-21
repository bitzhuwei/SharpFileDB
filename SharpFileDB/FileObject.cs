using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB
{
    /// <summary>
    /// Base class for all classed that can use CRUD in SharpFileDB.
    /// </summary>
    public abstract class FileObject
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; protected set; }

        public FileObject()
        {
            this.Id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return string.Format("Id: {0}", this.Id);
            //return base.ToString();
        }
    }
}
