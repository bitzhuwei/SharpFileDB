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
    }
}
