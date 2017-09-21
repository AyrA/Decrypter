using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Decrypter
{
    public partial class frmName : Form
    {
        public string Data
        { get; private set; }

        public frmName()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Data = tbName.Text.Trim();
        }
    }
}
