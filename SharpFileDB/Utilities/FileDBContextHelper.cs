using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Utilities
{
    /// <summary>
    /// <see cref="FileDBContext"/>类型的辅助类。
    /// </summary>
    public static class FileDBContextHelper
    {
        
        /// <summary>
        /// 获取数据库的文件流。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static FileStream GetFileStream(this FileDBContext db)
        {
            return db.fileStream;
        }

        /// <summary>
        /// 获取数据库的事务。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static Transaction GetTransaction(this FileDBContext db)
        {
            return db.transaction;
        }

        /// <summary>
        /// 获取数据库的头部。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static DBHeaderBlock GetDBHeaderBlock(this FileDBContext db)
        {
            return db.headerBlock;
        }

        /// <summary>
        /// 获取数据库表块的头结点。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static TableBlock GetTableBlockHeadNode(this FileDBContext db)
        {
            return db.tableBlockHead;
        }

        /// <summary>
        /// 从数据库文件中申请一定长度的空间。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="length"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<AllocatedSpace> Alloc(this FileDBContext db, long length, AllocPageTypes type)
        {
            // 由于一页能用的空间很有限，所以可能需要从多个页上获取空间。
            IList<AllocatedSpace> result = new List<AllocatedSpace>();

            FileStream fs = db.fileStream;

            long allocated = 0;
            while (allocated < length)
            {
                Int16 partLength = (length - allocated >= Consts.maxAvailableSpaceInPage) ? Consts.maxAvailableSpaceInPage : (Int16)(length - allocated);
                // 找出一个可用空间充足的指定类型的页。
                PageHeaderBlock page = PickPage(db, partLength, type);
                if (!db.transaction.affectedPages.ContainsKey(page.ThisPos))// 加入缓存备用。
                { db.transaction.affectedPages.Add(page.ThisPos, page); }
                AllocatedSpace item = new AllocatedSpace(page.ThisPos + Consts.pageSize - page.AvailableBytes - partLength, (Int16)length);
                result.Add(item);
                allocated += partLength;
            }

            return result;
        }

        /// <summary>
        /// 从文件数据库中找出一个符合条件的页。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="allocatingLength">希望申请到的字节数。</param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PageHeaderBlock PickPage(this FileDBContext db, Int16 allocatingLength, AllocPageTypes type)
        {
            PageHeaderBlock page;

            FileStream fs = db.fileStream;
            Transaction ts = db.transaction;
            DBHeaderBlock dbHeader = db.headerBlock;

            // 找到第一个给定类型的页的位置。
            long pagePos = dbHeader.GetPosOfFirstPage(type);

            if (pagePos == 0)// 尚无给定类型的页。
            {
                page = db.AllocEmptyPageOrNewPage();

                page.AvailableBytes -= allocatingLength;
                page.OccupiedBytes += allocatingLength;
                dbHeader.SetPosOfFirstPage(type, page.ThisPos);
            }
            else
            {
                // 最前面的table页的可用空间是最大的（这需要在后续操作中进行排序）
                PageHeaderBlock firstTablePage = GetTablePage(fs, ts, pagePos);

                if (firstTablePage.AvailableBytes >= allocatingLength)// 此页的空间足够用。
                {
                    // 把此页从Page链表中移除。
                    dbHeader.SetPosOfFirstPage(type, firstTablePage.NextPagePos);
                    firstTablePage.NextPagePos = 0;

                    page = firstTablePage;
                }
                else// 此页的空间不够用，还是要申请一个新页。
                {
                    page = db.AllocEmptyPageOrNewPage();
                }

                page.AvailableBytes -= allocatingLength;
                page.OccupiedBytes += allocatingLength;

                // 对申请的类型的页的链表进行排序。呼应上面的排序需求。
                SortPage(type, page, fs, ts, dbHeader);
            }

            return page;
        }

        /// <summary>
        /// 获取指定位置所在的页。
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="ts"></param>
        /// <param name="pagePos"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PageHeaderBlock GetTablePage(FileStream fs, Transaction ts, long pagePos)
        {
            PageHeaderBlock firstTablePage;
            if (ts.affectedPages.ContainsKey(pagePos))
            { firstTablePage = ts.affectedPages[pagePos]; }
            else
            { firstTablePage = fs.ReadBlock<PageHeaderBlock>(pagePos); }
            return firstTablePage;
        }

        /// <summary>
        /// 给更新了的页进行排序。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <param name="fs"></param>
        /// <param name="ts"></param>
        /// <param name="dbHeader"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SortPage(AllocPageTypes type, PageHeaderBlock page, FileStream fs, Transaction ts, Blocks.DBHeaderBlock dbHeader)
        {
            long headPos = dbHeader.GetPosOfFirstPage(type);
            if (headPos == 0)// 一个页也没有。
            { dbHeader.SetPosOfFirstPage(type, page.ThisPos); }
            else
            {
                PageHeaderBlock head = GetPageHeaderBlock(fs, ts, headPos);
                if (page.AvailableBytes >= head.AvailableBytes)// 与第一个页进行比较。
                {
                    page.NextPagePos = head.ThisPos;
                    dbHeader.SetPosOfFirstPage(type, page.ThisPos);
                }
                else// 与后续的页进行比较。
                {
                    PageHeaderBlock current = head;
                    while (current.NextPagePos != 0)
                    {
                        PageHeaderBlock next = GetPageHeaderBlock(fs, ts, current.NextPagePos);
                        if (page.AvailableBytes >= next.AvailableBytes)
                        {
                            if (next.ThisPos != current.NextPagePos)
                            { throw new Exception(string.Format("this should not happen.")); }
                            page.NextPagePos = current.NextPagePos; // next.ThisPos;
                            current.NextPagePos = page.ThisPos;
                            break;
                        }
                        else
                        { current = next; }
                    }
                    if (current.NextPagePos == 0)
                    {
                        current.NextPagePos = page.ThisPos;
                    }

                    if (!ts.affectedPages.ContainsKey(current.ThisPos))
                    { ts.affectedPages.Add(current.ThisPos, current); }
                }
            }
        }

        private static PageHeaderBlock GetPageHeaderBlock(FileStream fs, Transaction ts, long pagePos)
        {
            PageHeaderBlock page;
            if (ts.affectedPages.ContainsKey(pagePos))
            { page = ts.affectedPages[pagePos]; }
            else
            { page = fs.ReadBlock<PageHeaderBlock>(pagePos); }
            return page;
        }

        /// <summary>
        /// 申请一个空白页或新页。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static PageHeaderBlock AllocEmptyPageOrNewPage(this FileDBContext db)
        {
            PageHeaderBlock page;

            FileStream fs = db.fileStream;
            DBHeaderBlock dbHeader = db.headerBlock;
            long emptyPagePos = dbHeader.FirstEmptyPagePos;
            if (emptyPagePos != 0)// 存在空白页，则使用此空白页。
            {
                PageHeaderBlock emptyPage = fs.ReadBlock<PageHeaderBlock>(emptyPagePos);
                // 从链表中去掉此空白页。
                dbHeader.FirstEmptyPagePos = emptyPage.NextPagePos;
                emptyPage.NextPagePos = 0;

                page = emptyPage;
            }
            else// 没有空白页，则拓展数据库文件，增加一个页的长度。
            {
                PageHeaderBlock newPage = new PageHeaderBlock();
                newPage.ThisPos = fs.Length;
                newPage.OccupiedBytes = (Int16)(Consts.pageSize - Consts.maxAvailableSpaceInPage);
                newPage.AvailableBytes = Consts.maxAvailableSpaceInPage;
                fs.SetLength(fs.Length + Consts.pageSize);

                page = newPage;
            }

            return page;
        }
    }


    /// <summary>
    /// 把从给定位置开始的给定长度作为空闲空间标记出来。 
    /// </summary>
    public class AllocatedSpace
    {
        /// <summary>
        /// 空闲空间的起始位置。
        /// </summary>
        public long position;

        /// <summary>
        /// 空闲空间的长度。
        /// </summary>
        public Int16 length;

        /// <summary>
        /// 把从给定位置开始的给定长度作为空闲空间标记出来。
        /// </summary>
        /// <param name="position"></param>
        /// <param name="length"></param>

        public AllocatedSpace(long position, Int16 length)
        {
            this.position = position;
            this.length = length;
        }

        /// <summary>
        /// 显示此对象的信息，便于调试。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", position, length);
        }
    }
}
