namespace SharpFileDB.VisualDesigner
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btnAddTable = new System.Windows.Forms.Button();
            this.lstTable = new System.Windows.Forms.ListBox();
            this.btnDeleteTable = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNamespace = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnAddTable
            // 
            this.btnAddTable.Location = new System.Drawing.Point(12, 39);
            this.btnAddTable.Name = "btnAddTable";
            this.btnAddTable.Size = new System.Drawing.Size(143, 23);
            this.btnAddTable.TabIndex = 3;
            this.btnAddTable.Text = "添加表(Add Table)";
            this.btnAddTable.UseVisualStyleBackColor = true;
            // 
            // lstTable
            // 
            this.lstTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTable.FormattingEnabled = true;
            this.lstTable.ItemHeight = 12;
            this.lstTable.Location = new System.Drawing.Point(12, 65);
            this.lstTable.Name = "lstTable";
            this.lstTable.Size = new System.Drawing.Size(774, 304);
            this.lstTable.TabIndex = 2;
            // 
            // btnDeleteTable
            // 
            this.btnDeleteTable.Location = new System.Drawing.Point(161, 39);
            this.btnDeleteTable.Name = "btnDeleteTable";
            this.btnDeleteTable.Size = new System.Drawing.Size(143, 23);
            this.btnDeleteTable.TabIndex = 3;
            this.btnDeleteTable.Text = "删除表(Delete Table)";
            this.btnDeleteTable.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "namespace:";
            // 
            // txtNamespace
            // 
            this.txtNamespace.Location = new System.Drawing.Point(81, 12);
            this.txtNamespace.Name = "txtNamespace";
            this.txtNamespace.Size = new System.Drawing.Size(170, 21);
            this.txtNamespace.TabIndex = 5;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 385);
            this.Controls.Add(this.txtNamespace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDeleteTable);
            this.Controls.Add(this.btnAddTable);
            this.Controls.Add(this.lstTable);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "SharpFileDB Designer by http://bitzhuwei.cnblogs.com";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddTable;
        private System.Windows.Forms.ListBox lstTable;
        private System.Windows.Forms.Button btnDeleteTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNamespace;
    }
}