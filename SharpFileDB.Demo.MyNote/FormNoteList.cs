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
    }
}
