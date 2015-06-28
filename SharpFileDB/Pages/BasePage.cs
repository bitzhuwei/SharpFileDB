﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileDB.Pages
{
    internal enum PageType : byte { Unknown, Empty, Header, Collection, Index, Data, Extend }

    /// <summary>
    /// Base type for pages.
    /// </summary>
    internal abstract class BasePage
    {
        #region Page Constants

        /// <summary>
        /// This size is used bytes in header pages 29 bytes
        /// </summary>
        public const UInt16 PAGE_HEADER_SIZE = 29;

        /// <summary>
        /// Bytes avaiable to store data removing page header size - 4079 bytes
        /// </summary>
        public const UInt16 PAGE_AVAILABLE_BYTES = PageAddress.PAGE_SIZE - PAGE_HEADER_SIZE;

        #endregion

        //TODO: uint.MaxValue * 4 * 1024 byte = 17592186040320 byte = 16383 GB = 15 TB
        // this to-do is done.
        /// <summary>
        /// Represent page number - start in 0 with HeaderPage [8 bytes]
        /// </summary>
        public UInt64 PageID { get; set; }

        /// <summary>
        /// Represent the previous page. Used for page-sequences - MaxValue represent that has NO previous page [8 bytes]
        /// </summary>
        public UInt64 PrevPageID { get; set; }

        /// <summary>
        /// Represent the next page. Used for page-sequences - MaxValue represent that has NO next page [8 bytes]
        /// </summary>
        public UInt64 NextPageID { get; set; }

        /// <summary>
        /// Indicate the page type [1 byte]
        /// </summary>
        public PageType PageType { get; set; }

        /// <summary>
        /// Used for all pages to count itens inside this page(bytes, nodes, blocks, ...)
        /// </summary>
        public UInt16 ItemCount { get; set; }

        /// <summary>
        /// Used to find a free page using only header search [used in FreeList]
        /// Its updated when a page modify content length (add/remove items)
        /// </summary>
        public UInt16 FreeBytes { get; set; }

        /// <summary>
        /// Indicate that this page is dirty (was modified) and must persist when commited [not-persistable]
        /// </summary>
        public bool IsDirty { get; set; }

        public BasePage()
        {
            this.PrevPageID = UInt64.MaxValue;
            this.NextPageID = UInt64.MaxValue;
            this.PageType = SharpFileDB.Pages.PageType.Unknown;
            this.ItemCount = 0;
            this.FreeBytes = PAGE_AVAILABLE_BYTES;
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
        /// Clear page content (using when delete a page)
        /// </summary>
        public virtual void Clear()
        {
            this.PrevPageID = UInt64.MaxValue;
            this.NextPageID = UInt64.MaxValue;
            this.PageType = PageType.Empty;
            this.ItemCount = 0;
            this.FreeBytes = PAGE_AVAILABLE_BYTES;
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

        public virtual void ReadHeader(BinaryReader reader)
        {
            this.PageID = reader.ReadUInt64();
            this.PrevPageID = reader.ReadUInt64();
            this.NextPageID = reader.ReadUInt64();
            this.PageType = (PageType)reader.ReadByte();
            this.ItemCount = reader.ReadUInt16();
            this.FreeBytes = reader.ReadUInt16();
        }

        public virtual void WriteHeader(BinaryWriter writer)
        {
            writer.Write(this.PageID);
            writer.Write(this.PrevPageID);
            writer.Write(this.NextPageID);
            writer.Write((byte)this.PageType);
            writer.Write(this.ItemCount);
            writer.Write(this.FreeBytes);
        }

        #endregion

        #region Page Content

        public virtual void ReadContent(BinaryReader reader)
        {
        }

        public virtual void WriteContent(BinaryWriter writer)
        {
        }

        #endregion
    }
}
