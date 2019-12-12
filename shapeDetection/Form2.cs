using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Media.Imaging;


namespace shapeDetection
{
    public partial class Form2 : Form
    {
        private Point Start;
        private Point Current;
        private bool isDrawing = false;
        private double X, Y, W, H;
        public Roulette_main m_fm2;

        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                // Get new position
                Current = e.Location;

                // Calculate rectangle cords/size
                X = Math.Min(Current.X, Start.X);
                Y = Math.Min(Current.Y, Start.Y);
                W = Math.Max(Current.X, Start.X) - X;
                H = Math.Max(Current.Y, Start.Y) - Y;

                panel1.Left = (int)X;
                panel1.Top = (int)Y;

                // Update rectangle
                panel1.Width = (int)W;
                panel1.Height = (int)H;
                panel1.Width = (int)W;
                panel1.Width = (int)W;
            }
        }

        private void Form2_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
            m_fm2.set_Screenshot_param((int)X, (int)Y, (int)W, (int)H);
            m_fm2.Show();
            this.Hide();
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            panel1.Width = 0; panel1.Height = 0;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        public Form2(Roulette_main sender)
        {
            m_fm2 = sender;
            m_fm2.Hide();
            InitializeComponent();
        }

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            Start = e.Location;
            m_fm2.Hide();
        }
    }
}
