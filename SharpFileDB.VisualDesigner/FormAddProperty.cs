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
    public partial class FormAddProperty : Form
    {
        public PropertyDesigner NewPropertyDesigner { get; set; }

        public FormAddProperty()
        {
            InitializeComponent();

            this.NewPropertyDesigner = new PropertyDesigner();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            {
                string type = this.txtType.Text.Trim();
                if (string.IsNullOrEmpty(type))
                {
                    string message = string.Format("{0}", "Please type in a valid type!");
                    MessageBox.Show(message);
                    return;
                }
            }
            {
                string name = this.txtName.Text.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    string message = string.Format("{0}", "Please type in a valid property name!");
                    MessageBox.Show(message);
                    return;
                }
            }
            {
                if (this.cmbIndex.SelectedItem == null)
                {
                    string message = string.Format("{0}", "Please select a valid index type!");
                    MessageBox.Show(message);
                    return;
                }
            }

            this.NewPropertyDesigner.PropertyType = this.txtType.Text.Trim();
            this.NewPropertyDesigner.PropertyName = this.txtName.Text.Trim();
            this.NewPropertyDesigner.IndexType = (IndexTypes)(this.cmbIndex.SelectedItem);
            this.NewPropertyDesigner.XmlNote = this.txtNote.Text.Trim();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void FormAddProperty_Load(object sender, EventArgs e)
        {
            foreach (var item in Enum.GetValues(typeof(IndexTypes)))
            {
                this.cmbIndex.Items.Add(item);
            }
        }
    }
}
