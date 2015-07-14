using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpFileDB.DebugHelper
{
    public class IndexInfo
    {
        private Blocks.IndexBlock indexBlock;

        public IndexInfo(Blocks.IndexBlock indexBlock)
        {
            this.indexBlock = indexBlock;

            this.IndexBindMember = indexBlock.BindMember;
        }

        public string IndexBindMember { get; set; }

        /// <summary>
        /// 显示此对象的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}", this.IndexBindMember);
            //return base.ToString();
        }
    }
}
