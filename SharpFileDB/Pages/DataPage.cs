﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpFileDB.Pages
{
    /// <summary>
    /// The DataPage thats stores object data.
    /// </summary>
    internal class DataPage : BasePage
    {
        ///// <summary>
        ///// If a Data Page has less that free space, it's considered full page for new items. Can be used only for update (DataPage) ~ 50% PAGE_AVAILABLE_BYTES
        ///// This value is used for minimize 
        ///// </summary>
        //public const int DATA_RESERVED_BYTES = PAGE_AVAILABLE_BYTES / 2;

        ///// <summary>
        ///// Returns all data blocks - Each block has one object
        ///// </summary>
        //public Dictionary<ushort, DataBlock> DataBlocks { get; set; }

        //public DataPage()
        //    : base()
        //{
        //    this.PageType = PageType.Data;
        //    this.DataBlocks = new Dictionary<ushort, DataBlock>();
        //}

        ///// <summary>
        ///// Clear page content - dataBlocks
        ///// </summary>
        //public override void Clear()
        //{
        //    base.Clear();
        //    this.DataBlocks = new Dictionary<ushort, DataBlock>();
        //}

        ///// <summary>
        ///// Update freebytes + items count
        ///// </summary>
        //public override void UpdateItemCount()
        //{
        //    this.ItemCount = (ushort)this.DataBlocks.Count;
        //    this.FreeBytes = PAGE_AVAILABLE_BYTES - this.DataBlocks.Sum(x => x.Value.Length);
        //}

        //public override void WriteContent(BinaryWriter writer)
        //{
        //    foreach (var block in this.DataBlocks.Values)
        //    {
        //        writer.Write(block.Position.Index);
        //        writer.Write(block.ExtendPageID);
        //        foreach (var idx in block.IndexRef)
        //        {
        //            writer.Write(idx);
        //        }
        //        writer.Write((ushort)block.Data.Length);
        //        writer.Write(block.Data);
        //    }
        //}

        //public override void ReadContent(BinaryReader reader)
        //{
        //    this.DataBlocks = new Dictionary<ushort, DataBlock>(ItemCount);

        //    for (var i = 0; i < ItemCount; i++)
        //    {
        //        var block = new DataBlock();

        //        block.Page = this;
        //        block.Position = new PageAddress(this.PageID, reader.ReadUInt16());
        //        block.ExtendPageID = reader.ReadUInt32();

        //        for(var j = 0; j < CollectionIndex.INDEX_PER_COLLECTION; j++)
        //        {
        //            block.IndexRef[j] = reader.ReadPageAddress();
        //        }

        //        var size = reader.ReadUInt16();
        //        block.Data = reader.ReadBytes(size);

        //        this.DataBlocks.Add(block.Position.Index, block);
        //    }            
        //}
    }
}
