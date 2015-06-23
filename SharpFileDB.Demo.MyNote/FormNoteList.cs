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

            string databaseDirectory = Path.Combine(Environment.CurrentDirectory, "noteDatabase");
            this.database = new FileDBContext(databaseDirectory, new DefaultPersistence(DefaultPersistence.PersistenceFormat.Soap));
        }

        private void FormNoteList_Load(object sender, EventArgs e)
        {
            UpdateAllNotes();
        }

        private void UpdateAllNotes()
        {
            Predicate<MyNote.Tables.Note> selectAll = new Predicate<MyNote.Tables.Note>(x => true);
            IList<MyNote.Tables.Note> noteList = this.database.Retrieve(selectAll);

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

                this.database.Create(note);

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

            UpdateUI();
        }

        private void UpdateUI()
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

            UpdateAllNotes();

            UpdateUI();
        }
    }
}
