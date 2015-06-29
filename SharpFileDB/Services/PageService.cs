using SharpFileDB.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharpFileDB.Services
{
    /// <summary>
    /// 
    /// </summary>
    internal class PageService
    {
        private DiskService _disk;
        private CacheService _cache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disk"></param>
        /// <param name="cache"></param>
        public PageService(DiskService disk, CacheService cache)
        {
            _disk = disk;
            _cache = cache;
        }

        /// <summary>
        /// Get a page from cache or from disk (and put on cache)
        /// </summary>
        public T GetPage<T>(UInt64 pageID)
            where T : PageBase, new()
        {
            var page = _cache.GetPage<T>(pageID);

            if (page == null)
            {
                page = _disk.ReadPage<T>(pageID);

                _cache.AddPage(page);
            }

            return page;
        }

        /// <summary>
        /// Read all sequences pages from a start pageID (using NextPageID) 
        /// </summary>
        public IEnumerable<T> GetSeqPages<T>(UInt64 firstPageID)
            where T : PageBase, new()
        {
            UInt64 currentPageID = firstPageID;

            while (currentPageID != uint.MaxValue)
            {
                var page = this.GetPage<T>(currentPageID);

                currentPageID = page.pageHeaderInfo.nextPageID;

                yield return page;
            }
        }

        public T GetEmptyPage<T>() where T : PageBase, new()
        {
            T page = new T();

        }

        /// <summary>
        /// Get a new empty page - can be a reused page (EmptyPage) or a clean one (extend datafile) 
        /// </summary>
        public T NewPage<T>(PageBase prevPage = null)
            where T : PageBase, new()
        {
            var page = new T();

            // try get page from Empty free list
            if (_cache.Header.FreeEmptyPageID != uint.MaxValue)
            {
                EmptyPage free = this.GetPage<EmptyPage>(_cache.Header.FreeEmptyPageID);

                // remove page from empty list
                this.AddOrRemoveToFreeList(false, free, _cache.Header, ref _cache.Header.FreeEmptyPageID);

                page.pageHeaderInfo.pageID = free.pageHeaderInfo.pageID;
            }
            else
            {
                page.pageHeaderInfo.pageID = ++_cache.Header.LastPageID;
            }

            // if there a page before, just fix NextPageID pointer
            if (prevPage != null)
            {
                page.pageHeaderInfo.previousPageID = prevPage.pageHeaderInfo.pageID;
                prevPage.pageHeaderInfo.nextPageID = page.pageHeaderInfo.pageID;
                prevPage.IsDirty = true;
            }

            // mark header and this new page as dirty, and then add to cache
            page.IsDirty = true;
            _cache.Header.IsDirty = true;

            _cache.AddPage(page);

            return page;
        }

        /// <summary>
        /// Delete an page using pageID - transform them in Empty Page and add to EmptyPageList
        /// </summary>
        public void DeletePage(UInt64 pageID, bool addSequence = false)
        {
            var pages = addSequence ? this.GetSeqPages<EmptyPage>(pageID).ToArray() : new EmptyPage[] { this.GetPage<EmptyPage>(pageID) };

            // Adding all pages to FreeList
            foreach (var page in pages)
            {
                // update page to mark as completly empty page
                page.Free();
                page.IsDirty = true;

                // add to empty free list
                this.AddOrRemoveToFreeList(true, page, _cache.Header, ref _cache.Header.FreeEmptyPageID);
            }
        }

        /// <summary>
        /// Returns a page that contains space enouth to data to insert new object - if not exits, create a new Page
        /// </summary>
        public T GetFreePage<T>(UInt64 startPageID, UInt16 size)
            where T : PageBase, new()
        {
            if (startPageID != uint.MaxValue)
            {
                // get the first page
                EmptyPage page = this.GetPage<EmptyPage>(startPageID);

                // check if there space in this page
                UInt16 free = page.pageHeaderInfo.freeBytes;

                // first, test if there is space on this page
                if (free >= size)
                {
                    return this.GetPage<T>(startPageID);
                }
            }

            // if not has space on first page, there is no page with space (pages are ordered), create a new one
            return this.NewPage<T>();
        }

        /// <summary>
        /// Add or Remove a page in a sequence
        /// </summary>
        /// <param name="add">Indicate that will add or remove from FreeList</param>
        /// <param name="page">Page to add or remove from FreeList</param>
        /// <param name="startPage">Page reference where start the header list node</param>
        /// <param name="fieldPageID">Field reference, from startPage</param>
        public void AddOrRemoveToFreeList(bool add, PageBase page, PageBase startPage, ref UInt64 fieldPageID)
        {
            if (add)
            {
                // if page has no prev/next it's not on list - lets add
                if (page.pageHeaderInfo.previousPageID == UInt64.MaxValue && page.pageHeaderInfo.nextPageID == uint.MaxValue)
                {
                    this.AddToFreeList(page, startPage, ref fieldPageID);
                }
                else
                {
                    // othersie this page is already in this list, lets move do put in free size desc order
                    this.MoveToFreeList(page, startPage, ref fieldPageID);
                }
            }
            else
            {
                // if this page is not in sequence, its not on freelist 
                if (page.pageHeaderInfo.previousPageID == uint.MaxValue && page.pageHeaderInfo.nextPageID == uint.MaxValue)
                    return;

                this.RemoveToFreeList(page, startPage, ref fieldPageID);
            }
        }

        /// <summary>
        /// Add a page in free list in desc free size order
        /// </summary>
        private void AddToFreeList(PageBase page, PageBase startPage, ref UInt64 fieldPageID)
        {
            var free = page.pageHeaderInfo.freeBytes;
            var nextPageID = fieldPageID;
            PageBase next = null;

            // let's page in desc order
            while (nextPageID != uint.MaxValue)
            {
                next = this.GetPage<EmptyPage>(nextPageID);

                if (free >= next.pageHeaderInfo.freeBytes)
                {
                    // assume my page in place of next page
                    page.pageHeaderInfo.previousPageID = next.pageHeaderInfo.previousPageID;
                    page.pageHeaderInfo.nextPageID = next.pageHeaderInfo.pageID;

                    // link next page to my page
                    next.pageHeaderInfo.previousPageID = page.pageHeaderInfo.pageID;
                    next.IsDirty = true;

                    // my page is the new first page on list
                    if (page.pageHeaderInfo.previousPageID == 0)
                    {
                        fieldPageID = page.pageHeaderInfo.pageID;
                        startPage.IsDirty = true;
                    }
                    else
                    {
                        // if not the first, ajust links from previous page
                        var prev = this.GetPage<EmptyPage>(page.pageHeaderInfo.previousPageID);
                        prev.pageHeaderInfo.nextPageID = page.pageHeaderInfo.pageID;
                        prev.IsDirty = true;
                    }

                    page.IsDirty = true;

                    return; // job done - exit
                }

                nextPageID = next.pageHeaderInfo.nextPageID;
            }

            // empty list, be the first
            if (next == null)
            {
                // it's first page on list
                page.pageHeaderInfo.previousPageID = 0;
                fieldPageID = page.pageHeaderInfo.pageID;
                startPage.IsDirty = true;
            }
            else
            {
                // it's last position on list (next = last page on list)
                page.pageHeaderInfo.previousPageID = next.pageHeaderInfo.pageID;
                next.pageHeaderInfo.nextPageID = page.pageHeaderInfo.pageID;
                next.IsDirty = true;
            }

            page.IsDirty = true;
        }

        /// <summary>
        /// Remove a page from list - the ease part
        /// </summary>
        private void RemoveToFreeList(PageBase page, PageBase startPage, ref UInt64 fieldPageID)
        {
            // this page is the first of list
            if (page.pageHeaderInfo.previousPageID == 0)
            {
                fieldPageID = page.pageHeaderInfo.nextPageID;
                startPage.IsDirty = true;
            }
            else
            {
                // if not the first, get previous page to remove NextPageId
                var prevPage = this.GetPage<EmptyPage>(page.pageHeaderInfo.previousPageID);
                prevPage.pageHeaderInfo.nextPageID = page.pageHeaderInfo.nextPageID;
                prevPage.IsDirty = true;
            }

            // if my page is not the last on sequence, ajust the last page
            if (page.pageHeaderInfo.nextPageID != uint.MaxValue)
            {
                var nextPage = this.GetPage<EmptyPage>(page.pageHeaderInfo.nextPageID);
                nextPage.pageHeaderInfo.previousPageID = page.pageHeaderInfo.previousPageID;
                nextPage.IsDirty = true;
            }

            page.pageHeaderInfo.previousPageID = page.pageHeaderInfo.nextPageID = uint.MaxValue;
            page.IsDirty = true;
        }

        /// <summary>
        /// When a page is already on a list it's more efficient just move comparing with sinblings
        /// </summary>
        private void MoveToFreeList(PageBase page, PageBase startPage, ref UInt64 fieldPageID)
        {
            //TODO: write a better solution
            this.RemoveToFreeList(page, startPage, ref fieldPageID);
            this.AddToFreeList(page, startPage, ref fieldPageID);
        }
    }
}
