using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 황금열쇠
{
    public partial class Form1 : Form
    {
        private readonly Thread thread;
        private GoldenKey GK;
        public string Key
        {
            get { return keyBox.Text; }
            set { keyBox.Text = value; }
        }
        public bool IsReady
        {
            get { return nextButton.Enabled; }
            set 
            { 
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

        private void NextButton_Click(object sender, EventArgs e)
        {
            goldenKeyLabel.Text = GK.goldenKeys.First();
            GK.goldenKeys.RemoveAt(0);
            if (GK.goldenKeys.Count == 0) nextButton.Enabled = false;
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
