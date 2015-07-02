using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Pages
{
    public struct PageHeaderInfo
    {
        #region Page Constants

        /// <summary>
        /// This size is used bytes in header pages 29 bytes
        /// </summary>
        public const UInt16 PAGE_HEADER_SIZE =
            sizeof(Int64) // public Int64 pageID;
            + sizeof(Int64) // public Int64 previousPageID;
            + sizeof(Int64) // public Int64 nextPageID;
            + sizeof(Byte) // public PageType pageType;
            + sizeof(UInt16) // public UInt16 itemCount;
            + sizeof(UInt16) // public UInt16 freeBytes;
            ; // 29;

        /// <summary>
        /// Bytes avaiable to store data removing page header size - 4079 bytes
        /// </summary>
        public const UInt16 PAGE_AVAILABLE_BYTES = PageAddress.PAGE_SIZE - PAGE_HEADER_SIZE;

        #endregion

        //public PageHeaderInfo()
        //{
        //    this.PageID = 0;
        //    this.PrevPageID = UInt64.MaxValue;
        //    this.NextPageID = UInt64.MaxValue;
        //    this.PageType = SharpFileDB.Pages.PageType.Unknown;
        //    this.ItemCount = 0;
        //    this.FreeBytes = PAGE_AVAILABLE_BYTES;
        //    this.IsDirty = false;
        //}

        //TODO: uint.MaxValue * 4 * 1024 byte = 17592186040320 byte = 16383 GB = 15 TB
        // this to-do is done.
        /// <summary>
        /// Represent page number - start in 0 with HeaderPage [8 bytes]
        /// </summary>
        public Int64 pageID;

        /// <summary>
        /// 页之间有双链表关系时，此值表示前一结点的page ID。
        /// Represent the previous page. Used for page-sequences - MaxValue represent that has NO previous page [8 bytes]
        /// </summary>
        public Int64 previousPageID;

        /// <summary>
        /// 页之间有双链表关系时，此值表示后一结点的page ID。
        /// Represent the next page. Used for page-sequences - MaxValue represent that has NO next page [8 bytes]
        /// </summary>
        public Int64 nextPageID;

        /// <summary>
        /// Indicate the page type [1 byte]
        /// </summary>
        public PageType pageType;

        /// <summary>
        /// Used for all pages to count itens inside this page(bytes, nodes, blocks, ...)
        /// </summary>
        public UInt16 itemCount;

        /// <summary>
        /// Used to find a free page using only header search [used in FreeList]
        /// Its updated when a page modify content length (add/remove items)
        /// </summary>
        public UInt16 freeBytes;

    }
}
