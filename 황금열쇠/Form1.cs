using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace 황금열쇠
{
    public partial class Form1 : Form
    {
        private readonly Thread thread;
        private readonly Random rnd = new Random();
        private GoldenKey GK;
        public bool ReadyBool = false;
        
        public string Key
        {
            get { return keyBox.Text; }
            set { keyBox.Text = value; }
        }

        public string Option
        {
            set
            {
                Color color = Color.FromArgb(255, rnd.Next(128, 256), rnd.Next(128, 256), rnd.Next(128, 256));
                wheel1.GetOption = (value, color);
                panel1.Controls.Add(new Label
                {
                    Text = (panel1.Controls.Count + 1).ToString() + ". " + value,
                    Font = new Font("강원교육모두 Bold", 16),
                    Location = new Point(0, 24 * panel1.Controls.Count),
                    Size = new Size(panel1.Width, panel1.Height / 20),
                    BackColor = color
                });
            }
        }

        public string Selected
        {
            set => goldenKeyLabel.Text = value;
        }

        public bool IsReady
        {
            get { return ReadyBool; }
            set 
            { 
                ReadyBool = value;
                WheelStopped = value;
                if (value) goldenKeyLabel.Text = "황금 열쇠 준비 완료!";
                else goldenKeyLabel.Text = "황금 열쇠 준비중";
            }
        }

        public bool WheelStopped
        {
            set
            {
                nextButton.Enabled = value;
                rerollButton.Enabled = value;
            }
        }

        public void RemoveOption(int index)
        {
            panel1.Controls[index].Dispose();
        }

        public Form1()
        {
            InitializeComponent();
            GK = new GoldenKey(this);
            GK.CheckCode();
            GK.Payload = string.Empty;
            thread = new Thread(new ThreadStart(GK.Connect));

            wheel1.parent = this;
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

        private void NextButton_Click(object sender, EventArgs e)
        {
            if (nextButton.Text == "다음 황금열쇠")
            {
                wheel1.RotateWheel = true;
                nextButton.Text = "멈추기";
                rerollButton.Enabled = false;
            }
            else
            {
                wheel1.RotateWheel = false;
                nextButton.Text = "다음 황금열쇠";
                nextButton.Enabled = false;
                GK.count -= 1;
            }
        }

        private void RerollButton_Click(object sender, EventArgs e)
        {
            wheel1.ResetWheel();
            panel1.Controls.Clear();
            IsReady = false;
            GK.count = 0;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }
    }
}
