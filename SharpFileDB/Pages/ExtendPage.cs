using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileDB.Pages
{
    /// <summary>
    /// Represent a extra data page that contains the object when is not possible store in DataPage (bigger then  PAGE_SIZE or on update has no more space on page)
    /// Can be used in sequence of pages to store big objects
    /// </summary>
    public class ExtendPage : PageBase
    {
        /// <summary>
        /// Represent the part or full of the object - if this page has NextPageID the object is bigger than this page
        /// </summary>
        public Byte[] Data { get; set; }

        public ExtendPage()
            : base(PageType.Extend)
        {
            this.Data = new Byte[0];
        }

        /// <summary>
        /// Clear page content - Data byte array
        /// </summary>
        public override void Free()
        {
            base.Free();
            this.Data = new Byte[0];
        }

        /// <summary>
        /// Update freebytes + items count
        /// </summary>
        public override void UpdateItemCount()
        {
            this.pageHeaderInfo.itemCount = (ushort)Data.Length;
            this.pageHeaderInfo.freeBytes = (UInt16)(PageHeaderInfo.PAGE_AVAILABLE_BYTES - this.Data.Length); // not used on ExtendPage
            
        }

        public override void ReadContent(BinaryReader reader)
        {
            this.Data = reader.ReadBytes(this.pageHeaderInfo.itemCount);
        }

        public override void WriteContent(BinaryWriter writer)
        {
            writer.Write(this.Data);
        }
    }
}
