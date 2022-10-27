using System;
using System.Windows.Forms;

namespace 황금열쇠
{
    public partial class Form3 : Form
    {
        private readonly Form1 parent;
        public Form3(Form1 window)
        {
            InitializeComponent();
            parent = window;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            parent.ReadOption(textBox1.Text);
            DialogResult = DialogResult.OK;
        }
    }
}
