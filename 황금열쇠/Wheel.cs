using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace 황금열쇠
{
    public partial class Wheel : UserControl
    {
        public Form1 parent;
        private Graphics g;
        private Rectangle rect;
        private readonly Random rnd = new Random();
        private float angle = 0;
        private float diff = 50;
        private int index = 0;
        private bool IsStopped = false;
        public List<(string name, Color plateColor)> options = new List<(string, Color)>();

        public string GetOption
        {
            set
            {
                Color color = Color.FromArgb(255, rnd.Next(128, 256), rnd.Next(128, 256), rnd.Next(128, 256));
                options.Add((value, color));
                DrawWheel();
            }
        }

        public bool RotateWheel
        {
            set
            {
                if (value) timer1.Start();
                else IsStopped = true;
            }
        }

        public void ResetWheel()
        {
            options.Clear();
            UpdateRect();
        }

        public Wheel()
        {
            InitializeComponent();
            timer1.Interval = 1000 / 60;
        }

        private void UpdateRect()
        {
            if (g != null) g.Clear(BackColor);
            int x;
            if (Width > Height) x = Height;
            else x = Width;
            rect = new Rectangle((Width - x) / 2, (Height - x) / 2, x, x);
            if (options.Count > 0) DrawWheel();
        }

        private void DrawWheel()
        {
            float theta = angle;
            foreach (var option in options)
            {
                var brush = new SolidBrush(option.plateColor);
                g.FillPie(brush, rect, theta, 360F / options.Count);
                g.DrawPie(new Pen(Brushes.Black), rect, theta, 360F / options.Count);
                theta += 360F / options.Count;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (diff > 0)
            {
                angle -= diff;
                if (angle < 0) angle += 360;
                DrawWheel();
                index = (int)Math.Floor((360 - angle) / (360 / options.Count)) % options.Count;
                parent.Selected = options[index].name;
                if (IsStopped) diff -= (float)(1 / Math.PI);
            }
            else
            {
                timer1.Stop();
                IsStopped = false;
                diff = 50;
                options.RemoveAt(index);
                parent.RemoveOption(index);
                if (options.Count > 0)
                {
                    DrawWheel();
                    parent.WheelStopped = true;
                }
                else
                {
                    g.Clear(BackColor);
                    parent.ReadyBool = false;
                }
            }
        }

        private void Wheel_Paint(object sender, PaintEventArgs e)
        {
            g = CreateGraphics();
            UpdateRect();
        }

        private void Wheel_SizeChanged(object sender, EventArgs e)
        {
            UpdateRect();
        }
    }
}
