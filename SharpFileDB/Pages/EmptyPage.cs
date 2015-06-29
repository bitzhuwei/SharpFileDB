using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Pages
{
    /// <summary>
    /// An empty page is a page that is not in use.
    /// </summary>
    public class EmptyPage : BasePage
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
    }
}
