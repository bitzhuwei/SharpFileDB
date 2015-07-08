using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpFileDB.SharpFileDBHelper
{
    public class IndexInfo
    {
        private Blocks.IndexBlock indexBlock;

        public IndexInfo(Blocks.IndexBlock indexBlock)
        {
            // TODO: Complete member initialization
            this.indexBlock = indexBlock;

            this.IndexBindMember = indexBlock.BindMember;
        }

        public string IndexBindMember { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", this.IndexBindMember);
            //return base.ToString();
        }
    }
}
