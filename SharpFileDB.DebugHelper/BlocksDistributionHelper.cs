using SharpFileDB.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFileDB.Utilities;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SharpFileDB.DebugHelper
{
    public static class BlocksDistributionHelper
    {
        private static Int16 GetMinBlockLength()
        {
            Int16 min = Consts.pageHeaderBlockLength;

            if (min > Consts.dbHeaderBlockLength) { min = Consts.dbHeaderBlockLength; }
            if (min > Consts.tableBlockLength) { min = Consts.tableBlockLength; }
            if (min > Consts.indexBlockLength) { min = Consts.indexBlockLength; }
            if (min > Consts.skipListNodeBlockLength) { min = Consts.skipListNodeBlockLength; }
            if (min > Consts.dataBlockLength) { min = Consts.dataBlockLength; }

            return min;
        }

        /// <summary>
        /// BMP位图中一个块所占的最小像素长度。
        /// </summary>
        const int blockLengthInBMP = 5;

        const int bmpWidth = 4096;
        const int bmpHeight = 100;

        static readonly Pen dbHeaderPen = new Pen(Color.Yellow);

        static readonly Pen tablePen = new Pen(Color.Blue);

        static readonly Pen indexPen = new Pen(Color.Black);

        static readonly Pen skipListNodePen = new Pen(Color.Purple);

        static readonly Pen dataBlockPen = new Pen(Color.Gold);

        //

        //

        static readonly Pen boundPen = new Pen(Color.Red);
        public static void BlocksShot(this FileDBContext db, string directory, string prefix = "")
        {
            string dir = Path.Combine(directory, "BlocksDistribute");
            Directory.CreateDirectory(dir);
            FileStream fs = db.GetFileStream();
            long pageCount = fs.Length / Consts.pageSize;
            Bitmap[] bmps = new Bitmap[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                bmps[i] = new Bitmap(bmpWidth + 2, bmpHeight + 2);
                Graphics g = Graphics.FromImage(bmps[i]);
                g.DrawRectangle(boundPen, 0, 0, bmpWidth + 1, bmpHeight + 1);
            }

            // 画 page header
            DrawPageHeaders(fs, pageCount, bmps);
            // 画 db header
            DrawDBHeader(db, bmps);
            // 画 table
            DrawTables(db, fs, bmps);
            // 画 index
            DrawIndex(db, bmps);
            // 画 skip list node
            DrawSkipListNode(db, fs, bmps);
            // 画 datablock
            DrawDataBlocks(db, fs, bmps);

            for (int i = 0; i < bmps.Length; i++)
            {
                string fullname = Path.Combine(dir, prefix + string.Format("{0:000}", i) + ".bmp");
                bmps[i].Save(fullname);
            }
        }

        //static readonly Brush pageHeaderBrush = new LinearGradientBrush(new Rectangle(0, 0, Consts.pageHeaderBlockLength, bmpHeight), Color.Silver, Color.GreenYellow, 90);


        private static void DrawPageHeaders(FileStream fs, long pageCount, Bitmap[] bmps)
        {
            for (int i = 0; i < pageCount; i++)
            {
                long pos = i * Consts.pageSize;
                PageHeaderBlock page = fs.ReadBlock<PageHeaderBlock>(pos);
                long index = pos / Consts.pageSize;
                Bitmap bmp = bmps[index];
                Graphics g = Graphics.FromImage(bmp);
                int startPos = (int)(page.ThisPos - Consts.pageSize * index);
                int length = page.ToBytes().Length;
                Brush brush = new LinearGradientBrush(new Point(startPos, 0), new Point(startPos + length, 0), Color.YellowGreen, Color.GreenYellow);
                g.FillRectangle(brush, startPos, 1, length, bmpHeight - 1);
                g.DrawRectangle(boundPen, startPos, 1, length, bmpHeight - 1);
                g.Dispose();
            }
        }

        //static readonly Brush dbHeaderBrush = new LinearGradientBrush(new Rectangle(0, 0, Consts.dbHeaderBlockLength, bmpHeight), Color.Silver, Color.GreenYellow, 270f);

        private static void DrawDBHeader(FileDBContext db, Bitmap[] bmps)
        {
            DBHeaderBlock dbHeader = db.GetDBHeaderBlock();
            long index = 0;
            Graphics g = Graphics.FromImage(bmps[index]);
            int startPos = (int)(dbHeader.ThisPos - Consts.pageSize * index);
            int length = dbHeader.ToBytes().Length;
            Brush brush = new LinearGradientBrush(new Point(startPos, 0), new Point(startPos + length, 0), Color.Purple, Color.Aqua);
            g.FillRectangle(brush, startPos, 1, length, bmpHeight - 1);
            g.DrawRectangle(boundPen, startPos, 1, length, bmpHeight - 1);
            g.Dispose();
        }


        //static readonly Brush tableBrush = new LinearGradientBrush(new Rectangle(0, 0, Consts.tableBlockLength, bmpHeight), Color.LightBlue, Color.DarkBlue, 0f);

        private static void DrawTables(FileDBContext db, FileStream fs, Bitmap[] bmps)
        {
            TableBlock tableBlockHead = db.GetTableBlockHeadNode();
            TableBlock previousTable = tableBlockHead;
            {
                long index = previousTable.ThisPos.PagePos() / Consts.pageSize;
                Bitmap bmp = bmps[index];
                Graphics g = Graphics.FromImage(bmp);
                int length = previousTable.ToBytes().Length;
                int startPos = (int)(previousTable.ThisPos - Consts.pageSize * index);
                Brush brush = new LinearGradientBrush(new Point(startPos, 0), new Point(startPos + length), Color.DarkOrange, Color.OrangeRed);
                g.FillRectangle(brush, startPos, 1, length, bmpHeight - 1);
                g.DrawRectangle(boundPen, startPos, 1, length, bmpHeight - 1);
                g.Dispose();
            }
            while (previousTable.NextPos != 0)
            {
                previousTable.TryLoadNextObj(fs);
                TableBlock table = previousTable.NextObj;

                long index = table.ThisPos.PagePos() / Consts.pageSize;
                Bitmap bmp = bmps[index];
                Graphics g = Graphics.FromImage(bmp);
                int length = table.ToBytes().Length;
                int startPos = (int)(table.ThisPos - Consts.pageSize * index);
                Brush brush = new LinearGradientBrush(new Point(startPos, 0), new Point(startPos + length, 0), Color.DarkOrange, Color.OrangeRed);
                g.FillRectangle(brush, startPos, 1, length, bmpHeight - 1);
                g.DrawRectangle(boundPen, startPos, 1, length, bmpHeight - 1);
                g.Dispose();

                previousTable = table;
            }
        }


        //static readonly Brush indexBrush = new LinearGradientBrush(new Rectangle(0, 0, Consts.indexBlockLength, bmpHeight), Color.DarkGoldenrod, Color.LightGoldenrodYellow, 0f);

        private static void DrawIndex(FileDBContext db, Bitmap[] bmps)
        {
            TableBlock tableBlockHead = db.GetTableBlockHeadNode();
            TableBlock previousTable = tableBlockHead;
            while (previousTable.NextPos != 0)
            {
                //previousTable.TryLoadNextObj(fs);
                TableBlock table = previousTable.NextObj;

                IndexBlock currentIndex = table.IndexBlockHead;
                while (currentIndex!=null)
                {
                    long index = currentIndex.ThisPos.PagePos() / Consts.pageSize;
                    Bitmap bmp = bmps[index];
                    Graphics g = Graphics.FromImage(bmp);
                    int startPos = (int)(currentIndex.ThisPos - Consts.pageSize * index);
                    int length = currentIndex.ToBytes().Length;
                    Brush brush = new LinearGradientBrush(new Point(startPos, 0), new Point(startPos + length, 0), Color.DarkGray, Color.LightGray);
                    g.FillRectangle(brush, startPos, 1, length, bmpHeight - 1);
                    g.DrawRectangle(boundPen, startPos, 1, length, bmpHeight - 1);
                    g.Dispose();

                    currentIndex.TryLoadNextObj(db.GetFileStream());
                    currentIndex = currentIndex.NextObj;
                }
                //while (currentIndex.NextPos != 0)
                //{
                //    long index = currentIndex.NextObj.ThisPos.PagePos() / Consts.pageSize;
                //    Bitmap bmp = bmps[index];
                //    Graphics g = Graphics.FromImage(bmp);
                //    int startPos = (int)(currentIndex.NextObj.ThisPos - Consts.pageSize * index);
                //    int length = currentIndex.NextObj.ToBytes().Length;
                //    Brush brush = new LinearGradientBrush(new Point(startPos, 0), new Point(startPos + length, 0), Color.DarkGray, Color.LightGray);
                //    g.FillRectangle(brush, startPos, 1, length, bmpHeight - 1);
                //    g.DrawRectangle(boundPen, startPos, 1, length, bmpHeight - 1);
                //    g.Dispose();

                //    currentIndex = currentIndex.NextObj;
                //}

                previousTable = table;
            }
        }


        //static readonly Brush skipListNodeBrush = new LinearGradientBrush(new Rectangle(0, 0, Consts.skipListNodeBlockLength, bmpHeight), Color.LightSeaGreen, Color.DarkSeaGreen, 0f);

        private static void DrawSkipListNode(FileDBContext db, FileStream fs, Bitmap[] bmps)
        {
            TableBlock tableBlockHead = db.GetTableBlockHeadNode();
            TableBlock previousTable = tableBlockHead;
            while (previousTable.NextPos != 0)
            {
                TableBlock table = previousTable.NextObj;

                IndexBlock currentIndex = table.IndexBlockHead;
                while (currentIndex.NextPos != 0)
                {
                    foreach (var head in currentIndex.NextObj.SkipListHeadNodes)
                    {
                        SkipListNodeBlock currentNode = head;
                        while (currentNode != null)
                        {
                            long index = currentNode.ThisPos.PagePos() / Consts.pageSize;
                            Bitmap bmp = bmps[index];
                            Graphics g = Graphics.FromImage(bmp);
                            int startPos = (int)(currentNode.ThisPos - Consts.pageSize * index);
                            int length = currentNode.ToBytes().Length;
                            Brush brush = new LinearGradientBrush(new Point(startPos, 0), new Point(startPos + length, 0), Color.LightSeaGreen, Color.DarkSeaGreen);
                            g.FillRectangle(brush, startPos, 1, length, bmpHeight - 1);
                            g.DrawRectangle(boundPen, startPos, 1, length, bmpHeight - 1);
                            g.Dispose();

                            currentNode.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);
                            currentNode = currentNode.RightObj;
                        }
                    }

                    currentIndex = currentIndex.NextObj;
                }

                previousTable = table;
            }
        }


        private static void DrawDataBlocks(FileDBContext db, FileStream fs, Bitmap[] bmps)
        {
            TableBlock tableBlockHead = db.GetTableBlockHeadNode();
            TableBlock previousTable = tableBlockHead;
            while (previousTable.NextPos != 0)
            {
                TableBlock table = previousTable.NextObj;

                IndexBlock currentIndex = table.IndexBlockHead;
                while (currentIndex.NextObj != null)
                {
                    SkipListNodeBlock currentNode = currentIndex.NextObj.SkipListHeadNodes[0];
                    while (currentNode != null)
                    {
                        if (currentNode.KeyPos != 0)
                        {
                            currentNode.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.Key | SkipListNodeBlockLoadOptions.Value);
                            {
                                long index = currentNode.Key.ThisPos.PagePos() / Consts.pageSize;
                                Bitmap bmp = bmps[index];
                                Graphics g = Graphics.FromImage(bmp);
                                int startPos = (int)(currentNode.Key.ThisPos - Consts.pageSize * index);
                                int length = currentNode.Key.ToBytes().Length;
                                Brush brush = new LinearGradientBrush(new Point(startPos, 0), new Point(startPos + length, 0), Color.DarkGoldenrod, Color.LightGoldenrodYellow);
                                g.FillRectangle(brush, startPos, 1, length, bmpHeight - 1);
                                g.DrawRectangle(boundPen, startPos, 1, length, bmpHeight - 1);
                                g.Dispose();
                            }
                            foreach (var dataBlock in currentNode.Value)
                            {
                                long index = dataBlock.ThisPos.PagePos() / Consts.pageSize;
                                Bitmap bmp = bmps[index];
                                Graphics g = Graphics.FromImage(bmp);
                                int startPos = (int)(dataBlock.ThisPos - Consts.pageSize * index);
                                int length = dataBlock.ToBytes().Length;
                                Brush brush = new LinearGradientBrush(new Point(startPos, 0), new Point(startPos + length, 0), Color.DarkGoldenrod, Color.LightGoldenrodYellow);
                                g.FillRectangle(brush, startPos, 1, length, bmpHeight - 1);
                                g.DrawRectangle(boundPen, startPos, 1, length, bmpHeight - 1);
                                g.Dispose();
                            }
                        }

                        currentNode.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.RightObj);
                        currentNode = currentNode.RightObj;
                    }

                    currentIndex = currentIndex.NextObj;
                }

                previousTable = table;
            }
        }


    }
}