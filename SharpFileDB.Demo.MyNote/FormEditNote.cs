using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpFileDB.Demo.MyNote
{
    public partial class FormEditNote : Form
    {
        private Tables.Note note;

        public FormEditNote(MyNote.Tables.Note note)
        {
            InitializeComponent();
            this.note = note;

            this.txtTitle.Text = note.Title;
            this.txtContent.Text = note.Content;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            MyNote.Tables.Note note = this.note;

            note.Title = this.txtTitle.Text;
            note.Content = this.txtContent.Text;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
