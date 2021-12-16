using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathNalyzer
{
    public partial class MesgBoxForm : Form
    {
        string Output = "";
        public MesgBoxForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (Owner as Graph).LatestOutput = Output;
            Close();
        }

        private void Input_TextChanged(object sender, EventArgs e)
        {
            Output = Input.Text;
        }
    }
}
