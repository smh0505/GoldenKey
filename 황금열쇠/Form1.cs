using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 황금열쇠
{
    public partial class Form1 : Form
    {
        private readonly Thread thread;
        private GoldenKey GK;
        public bool ReadyBool = false;
        public string Key
        {
            get { return keyBox.Text; }
            set { keyBox.Text = value; }
        }
        public bool IsReady
        {
            get { return ReadyBool; }
            set 
            { 
                ReadyBool = value;
                nextButton.Enabled = value; 
                rerollButton.Enabled = value;
                if (value) goldenKeyLabel.Text = "황금 열쇠 준비 완료!";
                else goldenKeyLabel.Text = "황금 열쇠 준비중";
            }
        }

        public Form1()
        {
            InitializeComponent();
            GK = new GoldenKey(this);
            GK.CheckCode();
            GK.Payload = string.Empty;
            thread = new Thread(new ThreadStart(GK.Connect));
        }

        private void KeyBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(keyBox.Text))
                connectButton.Enabled = true;
            else connectButton.Enabled = false;
        }

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            keyBox.Enabled = false;
            connectButton.Enabled = false;
            await GK.LoadPayload(keyBox.Text);
            keyBox.Clear();

            if (!string.IsNullOrEmpty(GK.Payload))
            {
                thread.Start();
                MessageBox.Show("투네이션 연결에 성공했습니다.",
                    "황금열쇠");
            }
            else
            {
                MessageBox.Show("투네이션 연결에 실패했습니다.\n" +
                    "다시 시도해주세요.", "황금열쇠");
                keyBox.Enabled = true;
            }
        }

        private async void NextButton_Click(object sender, EventArgs e)
        {
            nextButton.Enabled = false;
            rerollButton.Enabled = false;

            Random rnd = new Random();
            int x = 0;
            int y = 0;
            int l = 10;

            while (l < 1500)
            {
                x = rnd.Next(0, GK.goldenKeys.Count);
                goldenKeyLabel.Text = GK.goldenKeys[x];
                await Task.Delay(l);
                l += (int)(10 * Math.Pow(1.5, y));
                y++;
            }

            GK.goldenKeys.RemoveAt(x);
            rerollButton.Enabled = true;
            if (GK.goldenKeys.Count == 0) nextButton.Enabled = false;
            else nextButton.Enabled = true;
        }

        private void RerollButton_Click(object sender, EventArgs e)
        {
            IsReady = false;
            GK.goldenKeys.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }
    }
}
