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
    class EmptyPage : BasePage
    {
        public EmptyPage()
        {
            this.PageType = Pages.PageType.Empty;
        }

        public override void UpdateItemCount()
        {
            this.ItemCount = 0;
            this.FreeBytes = PAGE_AVAILABLE_BYTES;
        }
    }
}
