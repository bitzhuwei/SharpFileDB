using SharpFileDB.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Services
{
    /// <summary>
    /// 对数据库文件进行读写操作。
    /// </summary>
    public class DiskService : IDisposable
    {
        private const int LOCK_POSITION = 0;

        /// <summary>
        /// 锁定操作的最长时间。
        /// </summary>
        public TimeSpan timeout = new TimeSpan(0, 0, 1, 0, 0);

        private FileStream fileStream;

        private BinaryReader binaryReader;
        private BinaryWriter binaryWriter;
        private Action lockDBAction;

        /// <summary>
        ///对数据库文件进行读写操作。 
        /// </summary>
        /// <param name="fullname">数据库文件名。</param>
        public DiskService(string fullname)
        {
            var stream = new FileStream(fullname, FileMode.Open, FileAccess.ReadWrite,
                FileShare.ReadWrite, Pages.PageAddress.PAGE_SIZE);
            this.fileStream = stream;
            this.binaryReader = new BinaryReader(stream);
            this.binaryWriter = new BinaryWriter(stream);
        }

        /// <summary>
        /// 读取数据库文件的一页。
        /// </summary>
        /// <typeparam name="T">页类型。</typeparam>
        /// <param name="pageID">页编号。[0 ~ 0x7FFFFFFFFFFFF] or [0 ~ 2251799813685247]</param>
        /// <returns></returns>
        public T ReadPage<T>(UInt64 pageID) where T : PageBase
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public void WritePage(PageBase page)
        {
            var stream = this.fileStream;
            long posStart, posEnd;
            checked
            {
                ulong start = (page.pageHeaderInfo.pageID * PageAddress.PAGE_SIZE);
                ulong end = start + PageAddress.PAGE_SIZE;
                if ((ulong)(long.MaxValue) < end)
                { throw new Exception(string.Format("{0} is too far away as a FileStream.Position", end)); }
                posStart = (long)start;
                posEnd = (long)end;
            }

            // position cursor
            if (stream.Position != posStart)
            {
                stream.Seek(posStart, SeekOrigin.Begin);
            }

            // write page header
            page.WriteHeader(this.binaryWriter);

            // write content 
            page.WriteContent(this.binaryWriter);

            // write with zero non-used page
            if (stream.Position < posEnd)
            {
                this.binaryWriter.Write(new byte[posEnd - stream.Position]);
            }

            // if page is dirty, clean up
            page.IsDirty = false;
        }


        /// <summary>
        /// Pre-allocate more disk space to fast write new pages on disk
        /// </summary>
        public void AllocateDiskSpace(Int64 length)
        {
            FileStream stream = this.fileStream;

            if (stream.Length < length)
            {
                stream.SetLength(length);
            }
        }

        #region Lock/Unlock functions

        /// <summary>
        /// Lock the datafile when start a begin transaction
        /// </summary>
        public void Lock()
        {
            if(this.lockDBAction == null)
            {
                this.lockDBAction = new Action(this.LockDB);
            }

            TryExec(this.timeout, this.lockDBAction);
        }

        void LockDB()
        {
            FileStream stream = this.fileStream;

            // try to lock - if is in use, a exception will be throwed
            stream.Lock(LOCK_POSITION, 1);
        }

        /// <summary>
        /// Unlock the datafile
        /// </summary>
        public void UnLock()
        {
            FileStream stream = this.fileStream;

            stream.Unlock(LOCK_POSITION, 1);
        }

        #endregion Lock/Unlock functions


        /// <summary>
        /// Try execute a block of code until timeout when IO lock exception occurs OR access denind
        /// </summary>
        public static void TryExec(TimeSpan timeout, Action action)
        {
            var timer = DateTime.Now.Add(timeout);

            while (DateTime.Now < timer)
            {
                try
                {
                    action();
                    return;
                }
                catch (UnauthorizedAccessException)
                {
                    System.Threading.Thread.Sleep(250);
                }
                catch (IOException ex)
                {
                    ex.WaitIfLocked(250);
                }
            }

            throw new Exception(string.Format("Lock Time out {0}", timeout));
        }

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
            this.binaryReader.Close();
            this.binaryWriter.Close();
            this.fileStream.Close();

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
