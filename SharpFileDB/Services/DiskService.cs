using SharpFileDB.Blocks;
using SharpFileDB.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharpFileDB.Services
{
    /// <summary>
    /// 为数据库文件提供关于页的操作。
    /// </summary>
    public static class DiskService
    {

        /// <summary>
        /// 从数据库文件中申请一定长度的空间。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IList<AllocatedSpace> Alloc(this FileDBContext db, long length, AllocPageTypes type)
        {
            // 由于一页能用的空间很有限，所以可能需要从多个页上获取空间。
            IList<AllocatedSpace> result = new List<AllocatedSpace>();

            FileStream fs = db.fileStream;
            Blocks.DBHeaderBlock dbHeader = db.headerBlock;

            long allocated = 0;
            while (allocated < length)
            {
                Int16 partLength = (length - allocated >= Consts.maxAvailableSpaceInPage) ? Consts.maxAvailableSpaceInPage : (Int16)(length - allocated);
                // 找出一个可用空间充足的指定类型的页。
                PageHeaderBlock page = PickPage(db, partLength, type);
                AllocatedSpace item = new AllocatedSpace(page, (Int16)length);
                result.Add(item);
                allocated += partLength;
            }

            dbHeader.IsDirty = true;

            return result;
        }

        /// <summary>
        /// 从文件数据库中找出一个符合条件的页。
        /// </summary>
        /// <param name="db"></param>
        /// <param name="minAvailableBytes">页最少应具有的可用字节数。</param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static PageHeaderBlock PickPage(this FileDBContext db, Int16 minAvailableBytes, AllocPageTypes type)
        {
            PageHeaderBlock page;

            FileStream fs = db.fileStream;
            Blocks.DBHeaderBlock dbHeader = db.headerBlock;
            // 找到第一个给定类型的页的位置。
            long pagePos = dbHeader.GetPosOfFirstPage(type);

            if (pagePos == 0)// 尚无给定类型的页。
            {
                page = db.AllocEmptyPageOrNewPage();
                dbHeader.SetPosOfFirstPage(type, page.ThisPos);
            }
            else
            {
                // 最前面的table页的可用空间是最大的（这需要在某些地方进行排序）
                PageHeaderBlock firstTablePage = fs.ReadBlock<PageHeaderBlock>(pagePos);
                if (firstTablePage.AvailableBytes > minAvailableBytes)// 此页的空间足够用。
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
            }

            return page;
        }

        /// <summary>
        /// 申请一个空白页或新页。
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static PageHeaderBlock AllocEmptyPageOrNewPage(this FileDBContext db)
        {
            PageHeaderBlock block;

            FileStream fs = db.fileStream;
            DBHeaderBlock dbHeader = db.headerBlock;
            long emptyPagePos = dbHeader.FirstEmptyPagePos;
            if (emptyPagePos != 0)// 存在空白页，则使用此空白页。
            {
                PageHeaderBlock page = fs.ReadBlock<PageHeaderBlock>(emptyPagePos);
                // 从链表中去掉此空白页。
                dbHeader.FirstEmptyPagePos = page.NextPagePos;
                page.NextPagePos = 0;

                block = page;
            }
            else// 没有空白页，则拓展数据库文件，增加一个页的长度。
            {
                block = new PageHeaderBlock();
                block.ThisPos = fs.Length;
                block.OccupiedBytes = (Int16)(Consts.pageSize - Consts.maxAvailableSpaceInPage);
                block.AvailableBytes = Consts.maxAvailableSpaceInPage;
                fs.SetLength(fs.Length + Consts.pageSize);
            }

            return block;
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
        /// 从给定页申请到了指定长度的空闲空间。
        /// </summary>
        /// <param name="page"></param>
        /// <param name="length"></param>
        public AllocatedSpace(PageHeaderBlock page, Int16 length)
        {
            this.position = (page.ThisPos + (Consts.pageSize - page.AvailableBytes));
            this.length = length;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", position, length);
        }
    }
}
