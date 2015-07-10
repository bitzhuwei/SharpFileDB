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
            this.btnDeleteTable = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNamespace = new System.Windows.Forms.TextBox();
            this.btnAddProperty = new System.Windows.Forms.Button();
            this.btnDeleteProperty = new System.Windows.Forms.Button();
            this.btnGenerateCode = new System.Windows.Forms.Button();
            this.browseGeneratedCodeFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstTable = new System.Windows.Forms.ListBox();
            this.lstProperty = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.btnAddTable.Click += new System.EventHandler(this.btnAddTable_Click);
            // 
            // btnDeleteTable
            // 
            this.btnDeleteTable.Enabled = false;
            this.btnDeleteTable.Location = new System.Drawing.Point(161, 39);
            this.btnDeleteTable.Name = "btnDeleteTable";
            this.btnDeleteTable.Size = new System.Drawing.Size(143, 23);
            this.btnDeleteTable.TabIndex = 3;
            this.btnDeleteTable.Text = "删除表(Delete Table)";
            this.btnDeleteTable.UseVisualStyleBackColor = true;
            this.btnDeleteTable.Click += new System.EventHandler(this.btnDeleteTable_Click);
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
            this.txtNamespace.Text = "SharpFileDB.ExampleDB";
            // 
            // btnAddProperty
            // 
            this.btnAddProperty.Enabled = false;
            this.btnAddProperty.Location = new System.Drawing.Point(310, 39);
            this.btnAddProperty.Name = "btnAddProperty";
            this.btnAddProperty.Size = new System.Drawing.Size(173, 23);
            this.btnAddProperty.TabIndex = 3;
            this.btnAddProperty.Text = "添加属性(Add Property)";
            this.btnAddProperty.UseVisualStyleBackColor = true;
            this.btnAddProperty.Click += new System.EventHandler(this.btnAddProperty_Click);
            // 
            // btnDeleteProperty
            // 
            this.btnDeleteProperty.Enabled = false;
            this.btnDeleteProperty.Location = new System.Drawing.Point(489, 39);
            this.btnDeleteProperty.Name = "btnDeleteProperty";
            this.btnDeleteProperty.Size = new System.Drawing.Size(173, 23);
            this.btnDeleteProperty.TabIndex = 3;
            this.btnDeleteProperty.Text = "删除属性(Delete Property)";
            this.btnDeleteProperty.UseVisualStyleBackColor = true;
            this.btnDeleteProperty.Click += new System.EventHandler(this.btnDeleteProperty_Click);
            // 
            // btnGenerateCode
            // 
            this.btnGenerateCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerateCode.Location = new System.Drawing.Point(613, 12);
            this.btnGenerateCode.Name = "btnGenerateCode";
            this.btnGenerateCode.Size = new System.Drawing.Size(173, 21);
            this.btnGenerateCode.TabIndex = 3;
            this.btnGenerateCode.Text = "生成代码(Generate Code)";
            this.btnGenerateCode.UseVisualStyleBackColor = true;
            this.btnGenerateCode.Click += new System.EventHandler(this.btnGenerateCode_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 68);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstTable);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstProperty);
            this.splitContainer1.Size = new System.Drawing.Size(774, 305);
            this.splitContainer1.SplitterDistance = 258;
            this.splitContainer1.TabIndex = 6;
            // 
            // lstTable
            // 
            this.lstTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTable.FormattingEnabled = true;
            this.lstTable.ItemHeight = 12;
            this.lstTable.Location = new System.Drawing.Point(0, 0);
            this.lstTable.Name = "lstTable";
            this.lstTable.Size = new System.Drawing.Size(258, 305);
            this.lstTable.TabIndex = 4;
            this.lstTable.SelectedIndexChanged += new System.EventHandler(this.lstTable_SelectedIndexChanged);
            // 
            // lstProperty
            // 
            this.lstProperty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstProperty.FormattingEnabled = true;
            this.lstProperty.ItemHeight = 12;
            this.lstProperty.Location = new System.Drawing.Point(0, 0);
            this.lstProperty.Name = "lstProperty";
            this.lstProperty.Size = new System.Drawing.Size(512, 305);
            this.lstProperty.TabIndex = 4;
            this.lstProperty.SelectedIndexChanged += new System.EventHandler(this.lstProperty_SelectedIndexChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 385);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.txtNamespace);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGenerateCode);
            this.Controls.Add(this.btnDeleteProperty);
            this.Controls.Add(this.btnDeleteTable);
            this.Controls.Add(this.btnAddProperty);
            this.Controls.Add(this.btnAddTable);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "SharpFileDB Designer by http://bitzhuwei.cnblogs.com";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddTable;
        private System.Windows.Forms.Button btnDeleteTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.Button btnAddProperty;
        private System.Windows.Forms.Button btnDeleteProperty;
        private System.Windows.Forms.Button btnGenerateCode;
        private System.Windows.Forms.FolderBrowserDialog browseGeneratedCodeFolder;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lstTable;
        private System.Windows.Forms.ListBox lstProperty;
    }
}