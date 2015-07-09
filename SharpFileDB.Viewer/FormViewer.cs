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
            GetSkipListInfo(index, db, out skipList, out keyDict, out valueDict);

            if (keyDict.Count != valueDict.Count)
            { throw new Exception(); }

            SaveToImage(table, index, skipList, keyDict, valueDict, dir, db);

        }

        Font font = new Font("宋体",10);
        Brush brush = new SolidBrush(Color.Black);
        Pen boundPen = new Pen(Color.Red);
        Pen rightPosPen = new Pen(Color.DeepSkyBlue);

        private void SaveToImage(TableBlock table, IndexBlock index, List<List<SkipListNodeBlock>> skipList, Dictionary<long, DataBlock> keyDict, Dictionary<long, DataBlock> valueDict, string dir, FileDBContext db)
        {
            const int leftMargin = 25;
            const int topMargin = 13;
            const int nodeWidth = 150;
            const int nodeHeight = 30;
            const int arrowLength = 30;
            const int keyValueHeight = 20;
            const int keyValueInterval = 5;
            int width = skipList[skipList.Count - 1].Count * (nodeWidth + arrowLength) + leftMargin;
            int height = skipList.Count * (nodeHeight + arrowLength) + keyDict.Count * (keyValueHeight + keyValueInterval) + topMargin;
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
                    2, (i) * (nodeHeight + arrowLength) + 2);
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
                    graphics.DrawString(string.Format("[{0}: {1}, {2}]", node.ThisPos, node.KeyPos, node.ValuePos), font, brush,
                        leftMargin + widthStep * (nodeWidth + arrowLength),
                        i * (nodeHeight + arrowLength) + 2);
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
                        i * (nodeHeight + arrowLength),
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength),
                        i * (nodeHeight + arrowLength)
                        );
                    graphics.DrawLine(rightPosPen,
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength) - 5,
                        i * (nodeHeight + arrowLength) - 5,
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength),
                        i * (nodeHeight + arrowLength));
                    graphics.DrawLine(rightPosPen,
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength) - 5,
                        i * (nodeHeight + arrowLength) + 5,
                        leftMargin + rightWidthStep * (nodeWidth + arrowLength),
                        i * (nodeHeight + arrowLength));

                }
            }

            // 画边框。
            graphics.DrawRectangle(boundPen, 0, 0, width - 1, height - 1);

            string fullname = Path.Combine(dir, table.TableType.FullName + "-" + index.BindMember + ".bmp");
            bmp.Save(fullname);
            graphics.Dispose();
            bmp.Dispose();
        }

        private void GetSkipListInfo(IndexBlock indexBlock, FileDBContext db, out List<List<SkipListNodeBlock>> skipList, out Dictionary<long, DataBlock> keyDict, out Dictionary<long, DataBlock> valueDict)
        {
            FileStream fs = db.GetFileStream();

            skipList = new List<List<SkipListNodeBlock>>();
            Dictionary<long, SkipListNodeBlock> nodeDict = new Dictionary<long, SkipListNodeBlock>();
            keyDict = new Dictionary<long, DataBlock>();
            valueDict = new Dictionary<long, DataBlock>();

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
                    if (!nodeDict.ContainsKey(currentNode.ThisPos))
                    { nodeDict.Add(currentNode.ThisPos, currentNode); }
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

                currentHeadNodePos = nodeDict[currentHeadNodePos].DownPos;
            }
        }

    }
}
