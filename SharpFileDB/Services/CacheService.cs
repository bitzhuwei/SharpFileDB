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
    /// Represent all cache system and track dirty pages. All pages that load and need to be track for
    /// dirty (to be persist after) must be added in this class.
    /// </summary>
    internal class CacheService : IDisposable
    {
        /// <summary>
        /// a very simple dictionary for pages cache and track
        /// <para>(page.pageHeaderInfo.pageID, page)</para>
        /// </summary>
        private SortedDictionary<UInt64, PageBase> _cache;

        private DiskService _disk;

        private DBHeaderPage _header;

        public CacheService(DiskService disk)
        {
            _disk = disk;

            _cache = new SortedDictionary<UInt64, PageBase>();
        }

        /// <summary>
        /// Gets total pages in cache for database info
        /// </summary>
        public int PagesInCache { get { return _cache.Count; } }

        /// <summary>
        /// Get header page in cache or request for a new instance if not existis yet
        /// </summary>
        public DBHeaderPage Header
        {
            get
            {
                if (_header == null)
                    _header = _disk.ReadPage<DBHeaderPage>(0);
                return _header;
            }
        }

        /// <summary>
        /// Get a page inside cache system. Returns null if page not existis. 
        /// If T is more specific than page that I have in cache, returns null (eg. Page 2 is BasePage in cache and this method call for IndexPage PageId 2)
        /// </summary>
        public T GetPage<T>(UInt64 pageID)
            where T : PageBase
        {
            var page = _cache.GetOrDefault(pageID, null);

            //// if a need a specific page but has a PageBase, returns null
            //if (page != null && page.GetType() == typeof(PageBase) && typeof(T) != typeof(PageBase))
            //{
            //    return null;
            //}

            return (T)page;
        }

        /// <summary>
        /// Add a page to cache. if this page is in cache, override (except if is basePage - in this case, copy header)
        /// </summary>
        public void AddPage(PageBase page)
        {
            //_cache[page.pageHeaderInfo.pageID] = page;
            _cache.Add(page.pageHeaderInfo.pageID, page);
        }

        /// <summary>
        /// Empty cache and header page
        /// </summary>
        public void Clear(DBHeaderPage newHeaderPage)
        {
            _header = newHeaderPage;
            _cache.Clear();
        }

        /// <summary>
        /// Remove from cache only extend pages - useful for FileStorage
        /// </summary>
        public void RemoveExtendPages()
        {
            //var keys = _cache.Values.Where(x => x.pageHeaderInfo.pageType == PageType.Extend && x.IsDirty == false).Select(x => x.pageHeaderInfo.pageID);
            var keys = from item in _cache.Values
                       where item.pageHeaderInfo.pageType == PageType.Extend && item.IsDirty == false
                       select item.pageHeaderInfo.pageID;

            foreach (var key in keys)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// Persist all dirty pages
        /// </summary>
        public void PersistDirtyPages()
        {
            // alocate datafile file first (only when file need to grow)
            _disk.AllocateDiskSpace((long)((this.Header.LastPageID + 1) * PageAddress.PAGE_SIZE));

            foreach (var page in this.GetDirtyPages())
            {
                _disk.WritePage(page);
            }
        }

        /// <summary>
        /// Checks if cache has dirty pages
        /// </summary>
        public bool HasDirtyPages()
        {
            return this.GetDirtyPages().FirstOrDefault() != null;
        }

        /// <summary>
        /// Returns all dirty pages including header page (for better write performance, get all pages in PageID increase order)
        /// </summary>
        public IEnumerable<PageBase> GetDirtyPages()
        {
            if (this.Header.IsDirty)
            {
                yield return _header;
            }

            // now returns all pages in sequence
            var pages = from item in _cache.Values
                        where item.IsDirty
                        select item;
            foreach (var item in pages)
            {
                yield return item;
            }
        }

        //public void Dispose()
        //{
        //    this.Clear(null);
        //}


        #region IDisposable Members

        /// <summary>
        /// Internal variable which checks if Dispose has already been called
        /// </summary>
        private Boolean disposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(Boolean disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                //TODO: Managed cleanup code here, while managed refs still valid
            }
            //TODO: Unmanaged cleanup code here
            this.Clear(null);

            disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the private Dispose(bool) helper and indicate 
            // that we are explicitly disposing
            this.Dispose(true);

            // Tell the garbage collector that the object doesn't require any
            // cleanup when collected since Dispose was called explicitly.
            GC.SuppressFinalize(this);
        }

        #endregion
				
    }
}
