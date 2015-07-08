using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpFileDB.Demo.MyNote
{
    public partial class FormNoteList : Form
    {
        private FileDBContext database;
        private List<MyNote.Tables.Note> selectedNoteList = new List<Tables.Note>();

        public FormNoteList()
        {
            InitializeComponent();

            //string fullname = Path.Combine(@"C:\Users\DELL\Documents\百度云同步盘\SharpFileDB\SharpFileDB.Demo.MyNote\noteDatabase\note.db");
            string fullname = Path.Combine(Environment.CurrentDirectory, "MyNote.db");
            this.database = new FileDBContext(fullname);
        }

        private void FormNoteList_Load(object sender, EventArgs e)
        {
            UpdateAllNotes();
        }

        private void UpdateAllNotes()
        {
            IList<MyNote.Tables.Note> noteList;
            //Predicate<MyNote.Tables.Note> selectAll = new Predicate<MyNote.Tables.Note>(x => true);
            //IList<MyNote.Tables.Note> noteList = this.database.Find(selectAll);
            // 下面的方式都是可以的。
            //noteList = this.database.Find((MyNote.Tables.Note n) => true);
            //noteList = this.database.Find<MyNote.Tables.Note>(x => true); 
            noteList = this.database.FindAll<MyNote.Tables.Note>();

            this.lstNotes.Items.Clear();

            this.lstNotes.Items.AddRange(noteList.ToArray());
            this.lblNoteCount.Text = string.Format("{0} notes", noteList.Count);
        }

        private void btnAddNote_Click(object sender, EventArgs e)
        {
            FormAddNote frmAddNote = new FormAddNote();
            if (frmAddNote.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MyNote.Tables.Note note = frmAddNote.NewNote;

                this.database.Insert(note);

                this.UpdateAllNotes();
            }
        }

        private void lstNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lstNotes.SelectedIndex >= 0)
            {
                this.selectedNoteList.Clear();

                foreach (var item in this.lstNotes.SelectedItems)
                {
                    MyNote.Tables.Note note = item as MyNote.Tables.Note;
                    this.selectedNoteList.Add(note);
                }
            }

            UpdateSelectingUI();
        }

        private void UpdateSelectingUI()
        {

            this.lblSelectedCount.Text = string.Format("selecting {0}", this.selectedNoteList.Count);
            this.btnDeleteNode.Enabled = this.selectedNoteList.Count > 0;
        }

        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            foreach (var item in this.selectedNoteList)
            {
                this.database.Delete(item);
            }

            this.selectedNoteList.Clear();

            UpdateAllNotes();

            UpdateSelectingUI();
        }

        private void lstNotes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.lstNotes.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                MyNote.Tables.Note note = this.lstNotes.Items[index] as MyNote.Tables.Note;

                FormEditNote frmEditNote = new FormEditNote(note);
                if (frmEditNote.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    database.Update(note);

                    UpdateAllNotes();

                    UpdateSelectingUI();
                }
            }

        }

        private void lblSelectedCount_Click(object sender, EventArgs e)
        {
            string str = SharpFileDB.SharpFileDBHelper.SharpFileDBHelper.Print(this.database);
            string message = string.Format("{0}", str);
            //MessageBox.Show(message);
            (new FormTip(str)).Show();

        }
    }
}
