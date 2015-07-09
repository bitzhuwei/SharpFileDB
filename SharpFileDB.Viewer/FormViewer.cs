using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpFileDB.DebugHelper;
using SharpFileDB.Utilities;
using SharpFileDB.Blocks;
using System.IO;
using System.Diagnostics;

namespace SharpFileDB.Viewer
{
    public partial class FormViewer : Form
    {
        //private SharpFileDBInfo DBInfo;
        public FormViewer()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (this.openSharpFileDB.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtFullname.Text = this.openSharpFileDB.FileName;

                this.btnRefresh_Click(this.btnRefresh, e);

                this.btnRefresh.Enabled = true;
                this.btnDetail.Enabled = true;
                this.btnSkipLists.Enabled = true;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.lstTable.Items.Clear();
            this.lstIndex.Items.Clear();
            this.lstRecord.Items.Clear();

            using (SharpFileDB.FileDBContext db = new FileDBContext(this.txtFullname.Text, true))
            {
                SharpFileDBInfo dbInfo = db.GetDBInfo();
                foreach (TableInfo table in dbInfo.tableList)
                {
                    this.lstTable.Items.Add(table);
                }
            }
        }

        private void lstTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            TableInfo tableInfo = this.lstTable.SelectedItem as TableInfo;
            if (tableInfo != null)
            {
                this.lstIndex.Items.Clear();
                foreach (IndexInfo index in tableInfo.indexInfoList)
                {
                    this.lstIndex.Items.Add(index);
                }

                this.lstRecord.Items.Clear();
                foreach (Table record in tableInfo.recordList)
                {
                    this.lstRecord.Items.Add(record);
                }
            }

            //SharpFileDB.FileDBContext
            //SharpFileDB.ObjectId
            //SharpFileDB.Table
            //SharpFileDB.TableIndexAttribute
            //SharpFileDB.Blocks.?
            //SharpFileDB.Utilities.?
            //SharpFileDB.SharpFileDBHelper.SharpFileDBHelper
            //SharpFileDB.SharpFileDBHelper.SharpFileDBInfo
            //SharpFileDB.SharpFileDBHelper.TableInfo
            //SharpFileDB.SharpFileDBHelper.IndexInfo
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            string str = string.Empty;

            using (SharpFileDB.FileDBContext db = new FileDBContext(this.txtFullname.Text, true))
            {
                str = db.Print();
            }

            (new FormTip(str)).ShowDialog();
        }

        private void btnSkipLists_Click(object sender, EventArgs e)
        {
            string dir = Path.Combine(Environment.CurrentDirectory, "skiplists");
            Directory.CreateDirectory(dir);

            using (FileDBContext db = new FileDBContext(this.txtFullname.Text, true))
            {
                TableBlock tableBlockHead = db.GetTableBlockHeadNode();
                FileStream fs = db.GetFileStream();
                TableBlock previousTable = tableBlockHead;
                while (previousTable.NextPos != 0)
                {
                    previousTable.TryLoadNextObj(fs);
                    TableBlock table = previousTable.NextObj;
                    PrintTableBlock(table, dir, db);

                    previousTable = table;
                }
            }

            Process.Start("explorer", dir);
        }

        /// <summary>
        /// 把指定的表包含的所有索引的所有skip list node和key value信息都画到图片上。
        /// </summary>
        /// <param name="nextTable"></param>
        /// <param name="dir"></param>
        /// <param name="db"></param>
        private void PrintTableBlock(TableBlock table, string dir, FileDBContext db)
        {
            FileStream fs = db.GetFileStream();
            long indexBlockHeadPos = table.IndexBlockHeadPos;
            IndexBlock indexBlockHead = fs.ReadBlock<IndexBlock>(indexBlockHeadPos);
            IndexBlock previousIndex = indexBlockHead;
            while (previousIndex.NextPos != 0)
            {
                previousIndex.TryLoadNextObj(fs);
                IndexBlock index = previousIndex.NextObj;
                PrintIndexBlock(table, index, dir, db);

                previousIndex = index;
            }
        }

        private void PrintIndexBlock(TableBlock table, IndexBlock index, string dir, FileDBContext db)
        {
            List<List<SkipListNodeBlock>> skipList;
            Dictionary<long, DataBlock> keyDict;
            Dictionary<long, DataBlock> valueDict;
            Dictionary<long, SkipListNodeBlock> skipListNodeDict;

            GetSkipListInfo(index, db, out skipList, out keyDict, out valueDict, out skipListNodeDict);

            if (keyDict.Count != valueDict.Count)
            { throw new Exception(); }

            SaveToImage(table, index, skipList, keyDict, valueDict, skipListNodeDict, dir, db);

        }

        Font font = new Font("宋体",10);
        Brush brush = new SolidBrush(Color.Black);
        Pen boundPen = new Pen(Color.Red);
        Pen rightPosPen = new Pen(Color.DeepSkyBlue);
        Pen downPosPen = new Pen(Color.BlueViolet);

        private void SaveToImage(TableBlock table, IndexBlock index, List<List<SkipListNodeBlock>> skipList, Dictionary<long, DataBlock> keyDict, Dictionary<long, DataBlock> valueDict, Dictionary<long, SkipListNodeBlock> skipListNodeDict, string dir, FileDBContext db)
        {
            const int leftMargin = 25;
            const int topMargin = 13;
            const int nodeWidth = 200;
            const int nodeHeight = 30;
            const int arrowLength = 30;
            const int keyValueHeight = 20;
            //const int keyValueInterval = 5;
            const int bottomLargin = 30;

            int width = skipList[skipList.Count - 1].Count * (nodeWidth + arrowLength) + leftMargin;
            int height = skipList.Count * (nodeHeight + arrowLength) + keyDict.Count * keyValueHeight * 2 + topMargin + bottomLargin;
            Bitmap bmp = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bmp);

            int maxLevel = db.GetDBHeaderBlock().MaxLevelOfSkipList;
            if (maxLevel != skipList.Count) { throw new Exception(); }

            // 画 skip list node.
            List<SkipListNodeBlock> lastLine = skipList[skipList.Count - 1];

            for (int i = 0; i < skipList.Count; i++)
            {
                List<SkipListNodeBlock> line = skipList[i];
                graphics.DrawString(string.Format("{0}:", skipList.Count - i - 1), font, brush,
                    2, (i) * (nodeHeight + arrowLength) + topMargin);
                for (int j = 0; j < line.Count; j++)
                {
                    int widthStep = 0;
                    if (j + 1 == line.Count) { widthStep = lastLine.Count - 1; }
                    else
                    {
                        while (lastLine[widthStep].KeyPos != line[j].KeyPos)
                        {
                            widthStep++;
                        }
                    }
                    SkipListNodeBlock node = line[j];
                    graphics.DrawString(string.Format("[@[{0}]: {1}, {2}]", node.ThisPos, node.KeyPos, node.ValuePos), font, brush,
                        leftMargin + widthStep * (nodeWidth + arrowLength),
                        i * (nodeHeight + arrowLength) + topMargin);
                }
            }

            // 画RightPos箭头。
            for (int i = 0; i < skipList.Count; i++)
            {
                List<SkipListNodeBlock> line = skipList[i];
                for (int j = 0; j < line.Count - 1; j++)
                {
                    int leftWidthStep = 0, rightWidthStep = 0;
                    while (lastLine[leftWidthStep].KeyPos != line[j].KeyPos)
                    {
                        leftWidthStep++;
                    }
                    if (j + 1 + 1 == line.Count) { rightWidthStep = lastLine.Count - 1; }
                    else
                    {
                        while (lastLine[rightWidthStep].KeyPos != line[j + 1].KeyPos)
                        {
                            rightWidthStep++;
                        }
                    }
                    graphics.DrawLine(rightPosPen,
                        leftMargin + leftWidthStep * (nodeWidth + arrowLength) + nodeWidth / 2,
                        topMargin + i * (nodeHeight + arrowLength),
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength),
                        topMargin + i * (nodeHeight + arrowLength)
                        );
                    graphics.DrawLine(rightPosPen,
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength) - 5,
                        topMargin + i * (nodeHeight + arrowLength) - 5,
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength),
                        topMargin + i * (nodeHeight + arrowLength));
                    graphics.DrawLine(rightPosPen,
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength) - 5,
                        topMargin + i * (nodeHeight + arrowLength) + 5,
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength),
                        topMargin + i * (nodeHeight + arrowLength));

                }
            }

            // 画DownPos箭头。
            for (int i = 0; i < skipList.Count; i++)
            {
                List<SkipListNodeBlock> line = skipList[i];
                for (int j = 0; j < line.Count; j++)
                {
                    SkipListNodeBlock node = line[j];
                    if (node.DownPos != 0)
                    {
                        SkipListNodeBlock downNode = skipListNodeDict[node.DownPos];
                        int topWidthStep = 0, topHeightStep = 0, downWidthStep = 0, downHeightStep = 0;
                        while (lastLine[topWidthStep].KeyPos != node.KeyPos)
                        {
                            topWidthStep++;
                        }
                        topHeightStep = i;
                        while (lastLine[downWidthStep].KeyPos != downNode.KeyPos)
                        {
                            downWidthStep++;
                        }
                        for (int k = 0; k < skipList.Count; k++)
                        {
                            if (skipList[k].Contains(downNode))
                            {
                                downHeightStep = k;
                                break;
                            }
                        }
                        graphics.DrawLine(downPosPen,
                            leftMargin + topWidthStep * (nodeWidth + arrowLength) + nodeWidth / 2,
                            topMargin + topHeightStep * (nodeHeight + arrowLength) + nodeHeight / 2,
                            leftMargin + downWidthStep * (nodeWidth + arrowLength) + nodeWidth / 2,
                            topMargin + downHeightStep * (nodeHeight + arrowLength) - 2
                            );
                        graphics.DrawLine(downPosPen,
                            leftMargin + downWidthStep * (nodeWidth + arrowLength) + nodeWidth / 2 + 5,
                            topMargin + downHeightStep * (nodeHeight + arrowLength) - 2 - 5,
                            leftMargin + downWidthStep * (nodeWidth + arrowLength) + nodeWidth / 2,
                            topMargin + downHeightStep * (nodeHeight + arrowLength) - 2
                            );
                        graphics.DrawLine(downPosPen,
                            leftMargin + downWidthStep * (nodeWidth + arrowLength) + nodeWidth / 2 - 5,
                            topMargin + downHeightStep * (nodeHeight + arrowLength) - 2 - 5,
                            leftMargin + downWidthStep * (nodeWidth + arrowLength) + nodeWidth / 2,
                            topMargin + downHeightStep * (nodeHeight + arrowLength) - 2
                            );
                    }
                }

            }

            // 画Key和Value。
            for (int i = 0; i < lastLine.Count; i++)
            {
                SkipListNodeBlock node = lastLine[i];
                if ((node.KeyPos == 0 && node.ValuePos != 0) || (node.KeyPos != 0 && node.ValuePos == 0))
                { throw new Exception(); }
                if (node.KeyPos != 0)
                {
                    DataBlock keyDataBlock = keyDict[node.KeyPos];
                    IComparable key = keyDataBlock.GetObject<IComparable>(db.GetFileStream());
                    graphics.DrawString(string.Format("Key: @[{0}]: {{{1}}}", node.KeyPos, key),
                        font, brush, leftMargin,
                        (skipList.Count - 1) * (nodeHeight + arrowLength) + topMargin + (keyValueHeight) * i * 2);
                    DataBlock valueDataBlock = valueDict[node.ValuePos];
                    Table value = valueDataBlock.GetObject<Table>(db.GetFileStream());
                    graphics.DrawString(string.Format("Value: @[{0}]: {{{1}}}", node.ValuePos, value),
                        font, brush, leftMargin,
                        (skipList.Count - 1) * (nodeHeight + arrowLength) + topMargin + (keyValueHeight) * (i * 2 + 1));
                }
            }

            // 画边框。
            graphics.DrawRectangle(boundPen, 0, 0, width - 1, height - 1);

            string fullname = Path.Combine(dir, table.TableType.FullName + "-" + index.BindMember + ".bmp");
            bmp.Save(fullname);
            graphics.Dispose();
            bmp.Dispose();
        }

        private void GetSkipListInfo(IndexBlock indexBlock, FileDBContext db, out List<List<SkipListNodeBlock>> skipList, out Dictionary<long, DataBlock> keyDict, out Dictionary<long, DataBlock> valueDict, out Dictionary<long, SkipListNodeBlock> skipListNodeDict)
        {
            FileStream fs = db.GetFileStream();

            skipList = new List<List<SkipListNodeBlock>>();
            keyDict = new Dictionary<long, DataBlock>();
            valueDict = new Dictionary<long, DataBlock>();
            skipListNodeDict = new Dictionary<long, SkipListNodeBlock>();

            long headNodePos = indexBlock.SkipListHeadNodePos;
            long currentHeadNodePos = headNodePos;
            while (currentHeadNodePos != 0)
            {
                List<SkipListNodeBlock> line = new List<SkipListNodeBlock>();

                long currentNodePos = currentHeadNodePos;
                while (currentNodePos != 0)
                {
                    SkipListNodeBlock currentNode = fs.ReadBlock<SkipListNodeBlock>(currentNodePos);
                    line.Add(currentNode);

                    if (!skipListNodeDict.ContainsKey(currentNode.ThisPos))
                    { skipListNodeDict.Add(currentNode.ThisPos, currentNode); }
                    if (currentNode.KeyPos != 0)
                    {
                        if (!keyDict.ContainsKey(currentNode.KeyPos))
                        {
                            //currentNode.TryLoadProperties(fs, SkipListNodeBlockLoadOptions.Key);
                            DataBlock keyBlock = fs.ReadBlock<DataBlock>(currentNode.KeyPos);
                            keyDict.Add(currentNode.KeyPos, keyBlock);
                        }
                    }
                    if (currentNode.ValuePos != 0)
                    {
                        if (!valueDict.ContainsKey(currentNode.ValuePos))
                        {
                            DataBlock valueBlock = fs.ReadBlock<DataBlock>(currentNode.ValuePos);
                            valueDict.Add(currentNode.ValuePos, valueBlock);
                        }
                    }

                    currentNodePos = currentNode.RightPos;
                }

                skipList.Add(line);

                currentHeadNodePos = skipListNodeDict[currentHeadNodePos].DownPos;
            }
        }

    }
}
