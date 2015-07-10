using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpFileDB.VisualDesigner
{
    public partial class FormMain : Form
    {
        List<TableDesigner> tableDesignerList = new List<TableDesigner>();

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {
            FormAddTable frmAddTable = new FormAddTable();
            if (frmAddTable.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TableDesigner table = frmAddTable.NewTableDesigner;
                this.tableDesignerList.Add(table);
                this.lstTable.Items.Add(table);
            }
        }

        private void lstTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            TableDesigner table = this.lstTable.SelectedItem as TableDesigner;
            bool selected = table != null;
            if (selected)
            {
                this.lstProperty.Items.Clear();
                foreach (var item in table.PropertyDesignerList)
                {
                    this.lstProperty.Items.Add(item);
                }
            }

            this.btnDeleteTable.Enabled = selected;
            this.btnAddProperty.Enabled = selected;
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            TableDesigner table = this.lstTable.SelectedItem as TableDesigner;
            bool selected = table != null;
            if (selected)
            {
                this.tableDesignerList.Remove(table);
                this.lstTable.Items.Remove(table);
                this.btnDeleteTable.Enabled = false;
                this.btnAddProperty.Enabled = false;
            }
        }

        private void btnAddProperty_Click(object sender, EventArgs e)
        {
            FormAddProperty frmAddProperty = new FormAddProperty();
            if (frmAddProperty.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PropertyDesigner propertyDesigner = frmAddProperty.NewPropertyDesigner;
                TableDesigner table = this.lstTable.SelectedItem as TableDesigner;
                table.PropertyDesignerList.Add(propertyDesigner);
                this.lstProperty.Items.Add(propertyDesigner);
            }
        }

        private void lstProperty_SelectedIndexChanged(object sender, EventArgs e)
        {
            PropertyDesigner property = this.lstProperty.SelectedItem as PropertyDesigner;
            bool selected = property != null;

            this.btnDeleteProperty.Enabled = selected;
        }

        private void btnDeleteProperty_Click(object sender, EventArgs e)
        {
            PropertyDesigner property = this.lstProperty.SelectedItem as PropertyDesigner;
            TableDesigner table = this.lstTable.SelectedItem as TableDesigner;
            this.lstProperty.Items.Remove(property);
            table.PropertyDesignerList.Remove(property);
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            if (browseGeneratedCodeFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string directory = browseGeneratedCodeFolder.SelectedPath;

                foreach (var item in this.tableDesignerList)
                {
                    string code = item.ToCSharpCode(this.txtNamespace.Text.Trim());
                    string fullname = Path.Combine(directory, item.Name + ".cs");
                    File.WriteAllText(fullname, code);
                }

                Process.Start("explorer", directory);
            }
        }
    }
}
