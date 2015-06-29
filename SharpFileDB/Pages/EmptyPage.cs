using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Pages
{
    /// <summary>
    /// An empty page is a page that is not in use.
    /// </summary>
    public class EmptyPage : PageBase
    {
        public EmptyPage()
            : base(PageType.Empty)
        {
        }

        public override void UpdateItemCount()
        {
            this.pageHeaderInfo.itemCount = 0;
            this.pageHeaderInfo.freeBytes = PageHeaderInfo.PAGE_AVAILABLE_BYTES;
        }

        public override void ReadContent(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteContent(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

    }
}
