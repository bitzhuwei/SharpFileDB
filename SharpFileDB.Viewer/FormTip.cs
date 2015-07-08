using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpFileDB.Viewer
{
    public partial class FormTip : Form
    {
        public FormTip(string tip)
        {
            InitializeComponent();
            this.textBox1.Text = tip;
        }
    }
}
