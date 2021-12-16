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
    public partial class Graph : Form
    {
        HashSet<PointF> Points = new HashSet<PointF>();
        int GraphScale = 10;
        public string LatestOutput = "";
        List<double> X = new List<double>();
        List<double> Y = new List<double>();
        Dictionary<PointF, string> Descriptions = new Dictionary<PointF, string>();
        Point MousePos;
        int offset_x = 0;
        int offset_y = 0;
        public Graph()
        {
            InitializeComponent();
            MouseWheel += Graph_MouseWheel;
            TipLabel.Text = "";
        }

        private void Graph_MouseWheel(object sender, MouseEventArgs e)
        {
            GraphScale += e.Delta / Math.Abs(e.Delta);
            Refresh();
        }

        private void Graph_Paint(object sender, PaintEventArgs e)
        {
            List<PointF> Proccessed = new List<PointF>();
            Descriptions.Clear();
            Pen pen = new Pen(Color.Black);
            e.Graphics.DrawLine(pen, Width / 2 - 10 + offset_x, 0, Width / 2 - 10 + offset_x, Height);
            e.Graphics.DrawLine(pen, 0, Height / 2 - 25 + offset_y, Width, Height / 2 - 25 + offset_y);
            
            foreach (PointF newPoint in Points)
            {
                Proccessed.Add(new PointF(Width / 2 - 10 + (newPoint.X * GraphScale) + offset_x, Height / 2 - 25 - (newPoint.Y * GraphScale) + offset_y));
            }

            pen.Color = Color.Green;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                e.Graphics.DrawLine(pen, Proccessed[i], Proccessed[i + 1]);
            }

            pen.Color = Color.Red;
            int index = 0;
            foreach (PointF newPoint in Proccessed)
            {
                e.Graphics.FillRectangle(pen.Brush, newPoint.X - 2, newPoint.Y - 2, 5, 5);
                ToolTip tip = new ToolTip();
                try
                {
                    Descriptions.Add(new PointF(newPoint.X - 2, newPoint.Y - 2), Points.ElementAt(index).ToString());
                }
                catch { }
                index++;
            }
        }

        private void UpdatePoints(List<double> X, List<double> Y)
        {
            PointF[] Backup = new PointF[Points.Count];
            try
            {
                Points.CopyTo(Backup);
                Points.Clear();
                for (int index = 0; index < X.Count; index++)
                {
                    Points.Add(new PointF((float)X.ElementAt(index), (float)Y.ElementAt(index)));
                }
                Refresh();
            }
            catch {
                Points = Backup.ToHashSet();
            }
        }

        private void xToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MesgBoxForm msg = new MesgBoxForm();
            msg.ShowDialog(this);

            foreach (string name in Program.MainHwnd.VarNames)
            {
                if (name == LatestOutput)
                    X = Program.MainHwnd.VariableValues[name];
            }
            LatestOutput = "";
            UpdatePoints(X, Y);
        }

        private void yToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MesgBoxForm msg = new MesgBoxForm();
            msg.ShowDialog(this);

            foreach (string name in Program.MainHwnd.VarNames)
            {
                if (name == LatestOutput)
                    Y = Program.MainHwnd.VariableValues[name];
            }
            LatestOutput = "";
            UpdatePoints(X, Y);
        }

        private void масштабToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MesgBoxForm msg = new MesgBoxForm();
            msg.ShowDialog(this);

            if (LatestOutput != "") GraphScale = Convert.ToInt32(LatestOutput);
            LatestOutput = "";
            UpdatePoints(X, Y);
        }

        private void Graph_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            offset_x = 0;
            offset_y = 0;
            UpdatePoints(X, Y);
        }

        private void MainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
            {
                foreach (var value in Descriptions)
                {
                    if (value.Key.X - 2 < e.X && value.Key.X + 7 > e.X && value.Key.Y - 2 < e.Y && value.Key.Y + 7 > e.Y)
                    {
                        TipLabel.Text = value.Value;
                        return;
                    }
                }
                TipLabel.Text = "";
            }
            else if (e.Button == MouseButtons.Left)
            {
                offset_x -= Math.Sign(- e.X);
                offset_y -= Math.Sign( - e.Y);
                Cursor.Position = new Point(Cursor.Position.X - e.X, Cursor.Position.Y - e.Y);
                Invalidate();
                return;
            }
        }

        private void Graph_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Position = MousePos;
            Cursor.Show();
        }

        private void Graph_MouseDown(object sender, MouseEventArgs e)
        {
            MousePos = Cursor.Position;
            Cursor.Hide();
        }
    }
}
