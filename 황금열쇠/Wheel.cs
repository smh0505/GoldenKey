using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace 황금열쇠
{
    public partial class Wheel : UserControl
    {
        public Form1 parent;
        public Graphics g;
        private Rectangle rect;
        private float angle = 0;
        private float diff = 50;
        private Option target;
        private bool IsStopped = false;

        public bool RotateWheel
        {
            set
            {
                if (value) timer1.Start();
                else IsStopped = true;
            }
        }

        public Wheel()
        {
            InitializeComponent();
            timer1.Interval = 1000 / 60;
        }

        private void UpdateRect()
        {
            if (g != null) g.Dispose();
            g = CreateGraphics();
            g.Clear(BackColor);
            int x;
            if (Width > Height) x = Height;
            else x = Width;
            rect = new Rectangle((Width - x) / 2, (Height - x) / 2, x, x);
            DrawWheel();
        }

        public void DrawWheel()
        {
            float theta = angle;
            
            if (parent != null)
            {
                foreach (var option in parent.Options)
                {
                    var brush = new SolidBrush(option.color);
                    g.FillPie(brush, rect, theta, option.count * 360F / parent.Sum);
                    g.DrawPie(new Pen(Brushes.Black), rect, theta, option.count * 360F / parent.Sum);
                    theta += option.count * 360F / parent.Sum;
                }
            }
            
            Point[] triangle = new Point[3]
            {
                new Point(rect.Right + 10, rect.Top + (rect.Height / 2) - 20),
                new Point(rect.Right - 10, rect.Top + (rect.Height / 2)),
                new Point(rect.Right + 10, rect.Top + (rect.Height / 2) + 20),
            };
            byte[] triangleEdges = new byte[3]
            {
                (byte)PathPointType.Line,
                (byte)PathPointType.Line,
                (byte)PathPointType.Line
            };
            g.FillPath(Brushes.Black, new GraphicsPath(triangle, triangleEdges));
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (diff > 0)
            {
                angle -= diff;
                if (angle <= 0) angle += 360;
                DrawWheel();
                target = Output((int)Math.Floor((360F - angle) / (360F / parent.Sum)) % parent.Sum);
                parent.Selected = target.name;
                if (IsStopped) diff -= (float)(1 / Math.PI);
            }
            else
            {
                timer1.Stop();
                IsStopped = false;
                diff = 50;
                parent.RemoveOption(parent.Options.IndexOf(target));
                parent.IsReady = true;
                parent.FillOption();
                DrawWheel();
            }
        }

        private Option Output(int index)
        {
            Option temp = new Option(string.Empty, Color.Empty, 0);
            int current = 0;
            foreach (var x in parent.Options)
            {
                if (current > index) break;
                else
                {
                    temp = x;
                    current += x.count;
                }
            }
            return temp;
        }

        private void Wheel_Paint(object sender, PaintEventArgs e)
        {
            UpdateRect();
        }

        private void Wheel_SizeChanged(object sender, EventArgs e)
        {
            UpdateRect();
        }
    }
}
