namespace SharpFileDB.Demo.MyNote
{
    partial class FormNoteList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstNotes = new System.Windows.Forms.ListBox();
            this.btnAddNote = new System.Windows.Forms.Button();
            this.btnDeleteNode = new System.Windows.Forms.Button();
            this.lblNoteCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstNotes
            // 
            this.lstNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstNotes.FormattingEnabled = true;
            this.lstNotes.ItemHeight = 12;
            this.lstNotes.Location = new System.Drawing.Point(12, 9);
            this.lstNotes.Name = "lstNotes";
            this.lstNotes.Size = new System.Drawing.Size(398, 316);
            this.lstNotes.TabIndex = 0;
            // 
            // btnAddNote
            // 
            this.btnAddNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNote.Location = new System.Drawing.Point(227, 337);
            this.btnAddNote.Name = "btnAddNote";
            this.btnAddNote.Size = new System.Drawing.Size(102, 23);
            this.btnAddNote.TabIndex = 1;
            this.btnAddNote.Text = "Add Note...";
            this.btnAddNote.UseVisualStyleBackColor = true;
            // 
            // btnDeleteNode
            // 
            this.btnDeleteNode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteNode.Enabled = false;
            this.btnDeleteNode.Location = new System.Drawing.Point(335, 337);
            this.btnDeleteNode.Name = "btnDeleteNode";
            this.btnDeleteNode.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteNode.TabIndex = 1;
            this.btnDeleteNode.Text = "Delete";
            this.btnDeleteNode.UseVisualStyleBackColor = true;
            // 
            // lblNoteCount
            // 
            this.lblNoteCount.AutoSize = true;
            this.lblNoteCount.Location = new System.Drawing.Point(12, 342);
            this.lblNoteCount.Name = "lblNoteCount";
            this.lblNoteCount.Size = new System.Drawing.Size(71, 12);
            this.lblNoteCount.TabIndex = 2;
            this.lblNoteCount.Text = "Note Count:";
            // 
            // FormNoteList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 372);
            this.Controls.Add(this.lblNoteCount);
            this.Controls.Add(this.btnDeleteNode);
            this.Controls.Add(this.btnAddNote);
            this.Controls.Add(this.lstNotes);
            this.Name = "FormNoteList";
            this.Text = "便签列表(Note List)";
            this.Load += new System.EventHandler(this.FormNoteList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstNotes;
        private System.Windows.Forms.Button btnAddNote;
        private System.Windows.Forms.Button btnDeleteNode;
        private System.Windows.Forms.Label lblNoteCount;
    }
}