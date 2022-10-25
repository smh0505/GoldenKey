using System.Windows.Forms;

namespace 황금열쇠
{
    public partial class AlertForm : Form
    {
        public AlertForm()
        {
            InitializeComponent();
        }

        public string Output
        {
            set => outputText.Text = value;
        }
    }
}
