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
        private Graphics g;
        private Rectangle rect;
        private float angle = 0;
        private float diff = 50;
        private int index = 0;
        private bool IsStopped = false;
        public List<(string name, Color plateColor)> options = new List<(string, Color)>();

        public (string, Color) GetOption
        {
            set
            {
                options.Add(value);
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
            if (g != null) DrawWheel();
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
                DrawWheel();
                if (options.Count > 0) parent.WheelStopped = true;
                else parent.ReadyBool = false;                
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
