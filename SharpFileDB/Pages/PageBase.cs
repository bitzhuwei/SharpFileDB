using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileDB.Pages
{
    public enum PageType : byte { Empty, DBHeader, Table, Index, Data, Extend }

    /// <summary>
    /// Base type for pages.
    /// </summary>
    public abstract class PageBase
    {

        /// <summary>
        /// page header information for every page.
        /// </summary>
        internal PageHeaderInfo pageHeaderInfo = new PageHeaderInfo();

        /// <summary>
        /// Indicate that this page is dirty (was modified) and must persist when commited [not-persistable]
        /// </summary>
        public bool IsDirty;

        public PageBase(PageType pageType)
        {
            this.pageHeaderInfo.previousPageID = UInt64.MaxValue;
            this.pageHeaderInfo.nextPageID = UInt64.MaxValue;
            this.pageHeaderInfo.pageType = pageType;
            this.pageHeaderInfo.itemCount = 0;
            this.pageHeaderInfo.freeBytes = PageHeaderInfo.PAGE_AVAILABLE_BYTES;
        }

        /// <summary>
        /// Every page must imeplement this ItemCount + FreeBytes
        /// Must be called after Items are updates (insert/deletes) to keep variables ItemCount and FreeBytes synced
        /// </summary>
        public abstract void UpdateItemCount();
        //{
        //    this.ItemCount = 0;
        //    this.FreeBytes = PAGE_AVAILABLE_BYTES;
        //}

        /// <summary>
        /// Free this page by clearing the page content (using when delete a page)
        /// </summary>
        public virtual void Free()
        {
            this.pageHeaderInfo.previousPageID = UInt64.MaxValue;
            this.pageHeaderInfo.nextPageID = UInt64.MaxValue;
            this.pageHeaderInfo.pageType = SharpFileDB.Pages.PageType.Empty;
            this.pageHeaderInfo.itemCount = 0;
            this.pageHeaderInfo.freeBytes = PageHeaderInfo.PAGE_AVAILABLE_BYTES;
        }

        ///// <summary>
        ///// Create a new espefic page, copy all header content
        ///// </summary>
        //public T CopyTo<T>()
        //    where T : BasePage, new()
        //{
        //    var page = new T();
        //    page.PageID = this.PageID;
        //    page.PrevPageID = this.PrevPageID;
        //    page.NextPageID = this.NextPageID;
        //    page.PageType = this.PageType;
        //    page.ItemCount = this.ItemCount;
        //    page.FreeBytes = this.FreeBytes;
        //    page.IsDirty = this.IsDirty;

        //    return page;
        //}

        #region Page Header

        public void ReadHeader(BinaryReader reader)
        {
            //this.pageHeaderInfo.pageID = reader.ReadInt64();

            this.pageHeaderInfo.previousPageID = reader.ReadInt64();
            this.pageHeaderInfo.nextPageID = reader.ReadInt64();
            this.pageHeaderInfo.pageType = (PageType)reader.ReadByte();
            this.pageHeaderInfo.itemCount = reader.ReadUInt16();
            this.pageHeaderInfo.freeBytes = reader.ReadUInt16();
        }

        public void WriteHeader(BinaryWriter writer)
        {
            //writer.Write(this.pageHeaderInfo.pageID);

            writer.Write(this.pageHeaderInfo.previousPageID);
            writer.Write(this.pageHeaderInfo.nextPageID);
            writer.Write((Byte)this.pageHeaderInfo.pageType);
            writer.Write(this.pageHeaderInfo.itemCount);
            writer.Write(this.pageHeaderInfo.freeBytes);
        }

        #endregion

        #region Page Content

        public abstract void ReadContent(BinaryReader reader);

        public abstract void WriteContent(BinaryWriter writer);

        #endregion

    }
}
