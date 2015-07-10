using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpFileDB.VisualDesigner
{
    public partial class FormAddTable : Form
    {
        public TableDesigner NewTableDesigner { get; set; }

        public FormAddTable()
        {
            InitializeComponent();

            this.NewTableDesigner = new TableDesigner();
        }

        private void FormAddTable_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            {
                string name = this.txtTableName.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    string message = string.Format("{0}", "Table name cannot be empty!");
                    MessageBox.Show(message);
                    return;
                }
            }

            this.NewTableDesigner.Name = this.txtTableName.Text;
            this.NewTableDesigner.XmlNote = this.txtNote.Text;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
