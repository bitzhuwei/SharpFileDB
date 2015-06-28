using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileDB.Pages
{
    /// <summary>
    /// Represents a page adress.
    /// </summary>
    internal struct PageAddress
    {
        /// <summary>
        /// The size of each page in disk - 4096 is NTFS default
        /// Page's size: 4 * 1024 bytes.
        /// </summary>
        public const UInt16 PAGE_SIZE = 0x1000; // = 4 * 1024;

        /// <summary>
        /// FileStream.Length is a System.Int64, which means database file's max length is System.Int64.MaxValue.
        /// </summary>
        public const UInt64 MAX_PAGE_COUNT = System.Int64.MaxValue / PAGE_SIZE;


        //public const int SIZE = 6;

        /// <summary>
        /// An empty page address is a farther page's address which is next to the farthest page in FileStream.
        /// </summary>
        public static PageAddress Empty = new PageAddress(MAX_PAGE_COUNT, PAGE_SIZE);

        //private UInt64 _pageID;
        /// <summary>
        /// [0 ~ 0x7FFFFFFFFFFFF] or [0 ~ 2251799813685247]
        /// </summary>
        public UInt64 pageID;
        //{
        //    get { return _pageID; }
        //    set
        //    {
        //        if (value > BasePage.MAX_PAGE_COUNT)
        //        {
        //            throw new ArgumentOutOfRangeException(
        //                "PageID", string.Format("{0} is too big for a PageAddress.PageID.", value));
        //        }
        //        //else if (value < 0)
        //        //{
        //        //    throw new ArgumentOutOfRangeException(
        //        //        "PageID", string.Format("PageAddress.PageID should not be a negtive value {0}.", value));
        //        //}

        //        _pageID = value;
        //    }
        //}

        //private UInt16 _index;
        /// <summary>
        /// [0 ~ 0x0FFF] or [0 ~ 4095]
        /// </summary>
        public UInt16 indexInPage;
        //{
        //    get { return _index; }
        //    set
        //    {
        //        if (value > BasePage.PAGE_SIZE)
        //        {
        //            throw new ArgumentOutOfRangeException(
        //                "PageID", string.Format("{0} is too big for a PageAddress..", value));
        //        }

        //        _index = value;
        //    }
        //}

        public bool IsEmpty
        {
            get { return pageID == MAX_PAGE_COUNT; }
        }

        public override bool Equals(object obj)
        {
            var other = (PageAddress)obj;
            return this.pageID == other.pageID && this.indexInPage == other.indexInPage;
        }

        public override int GetHashCode()
        {
            return (this.pageID + this.indexInPage).GetHashCode();
        }

        /// <summary>
        /// Represents a page adress.
        /// </summary>
        /// <param name="pageID">[0 ~ 0x7FFFFFFFFFFFF] or [0 ~ 2251799813685247]</param>
        /// <param name="indexInPage">[0 ~ 0x0FFF] or [0 ~ 4095]</param>
        public PageAddress(UInt64 pageID, UInt16 indexInPage)
        {
            this.pageID = pageID;
            this.indexInPage = indexInPage;
        }

        public override string ToString()
        {
            string result = string.Format("{0}: {1}{2}{3}{4}", this.pageID, this.indexInPage,
                this.IsEmpty ? " Empty" : "",
                this.pageID > MAX_PAGE_COUNT ? " error: PageID > MAX_PAGE_COUNT" : "",
                this.indexInPage > PAGE_SIZE ? " erro: IndexInPage > PAGE_SIZE" : "");
            return result;
        }
    }
}
