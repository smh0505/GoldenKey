using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace 황금열쇠
{
    public struct Option
    {
        public string name;
        public Color color;
        public int count;

        public Option(string name, Color color, int count)
        {
            this.name = name;
            this.color = color;
            this.count = count;
        }
    }

    public partial class Form1 : Form
    {
        private readonly Thread thread;
        private readonly Random rnd = new Random();
        private readonly GoldenKey GK;
        private readonly AlertForm AF;
        public bool ReadyBool = false;
        public List<Option> Options = new List<Option>();
        
        public string Key { set => keyBox.Text = value; }
        public string Selected { set => goldenKeyLabel.Text = value; }
        public int Sum
        {
            get
            {
                var sum = 0;
                foreach (var option in Options) sum += option.count;
                return sum;
            }
        }
        public bool IsReady
        {
            set
            {
                nextButton.Enabled = value;
                rerollButton.Enabled = value;
            }
        }

        private void NewOption(string name)
        {
            var color = Color.FromArgb(255, rnd.Next(128, 256), rnd.Next(128, 256), rnd.Next(128, 256));
            var newOption = new Option(name, color, 1);
            Options.Add(newOption);

            panel1.Controls.Add(new Label
            {
                Text = name + " * 1",
                Font = new Font("강원교육모두 Bold", 16),
                Location = new Point(0, 30 * panel1.Controls.Count),
                Size = new Size(panel1.Width, 30),
                BackColor = color,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            });
        }

        public void AddOption(string name)
        {
            int id = -1;
            foreach (var option in Options)
                if (option.name == name) id = Options.IndexOf(option);

            if (Options.Count == 0) NewOption(name);
            else if (id == -1) NewOption(name);
            else
            {
                var newOption = new Option(Options[id].name, Options[id].color, Options[id].count + 1);
                Options.RemoveAt(id);
                Options.Insert(id, newOption);
                panel1.Controls[id].Text = Options[id].name + " * " + Options[id].count.ToString();
            }
            
            wheel1.DrawWheel();
        }

        public void RemoveOption(int index)
        {
            if (AF != null)
            {
                AF.Output = Options[index].name;
                AF.ShowDialog();
            }

            var newOption = new Option(Options[index].name, Options[index].color, Options[index].count - 1);
            Options.RemoveAt(index);
            if (newOption.count > 0)
            {
                Options.Insert(index, newOption);
                panel1.Controls[index].Text = newOption.name + " * " + newOption.count.ToString();
            }
            else
            {
                panel1.Controls[index].Dispose();
                foreach (Control item in panel1.Controls) item.Location = new Point(0, 30 * panel1.Controls.IndexOf(item));
            }
        }

        public Form1()
        {
            InitializeComponent();
            GK = new GoldenKey(this);
            GK.CheckCode();
            GK.Payload = string.Empty;
            thread = new Thread(new ThreadStart(GK.Connect));

            wheel1.parent = this;
            AF = new AlertForm();
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
                splitContainer1.Panel2Collapsed = true;
                rerollButton.Enabled = true;
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
            }
        }

        private void RerollButton_Click(object sender, EventArgs e)
        {
            if (rerollButton.Text == "그만 받기")
            {
                rerollButton.Text = "계속 받기";
                nextButton.Enabled = true;
                ReadyBool = true;
            }
            else
            {
                rerollButton.Text = "그만 받기";
                nextButton.Enabled = false;
                ReadyBool = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }
    }
}
