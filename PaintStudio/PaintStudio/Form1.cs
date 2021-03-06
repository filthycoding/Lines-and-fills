﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace PaintStudio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static Stopwatch st = new Stopwatch();
        static bool DoublePoints = false;
        static bool ISDrawn = false;
        static Point p1 = new Point();
        static Point p2 = new Point();
        static int FillMethod = 0;
        static Color c;
        static Action a = null;

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(FillMethod > 0)
            {
            st.Reset();
            st.Start();
            }

            switch (FillMethod)
            {
                case 1:
                    FloodFill((Bitmap)pictureBox1.Image, e.Location, c);
                    break;
                case 2:
                    FloodFillQueue((Bitmap)pictureBox1.Image, e.Location, c);
                    break;
                case 3:
                    ScanFill((Bitmap)pictureBox1.Image, e.Location, c);
                    break;
                case 4:
                    ScanFillQueue((Bitmap)pictureBox1.Image, e.Location, c);
                    break;
                default:
                    if (ISDrawn)
                    {
                        if (p1.IsEmpty)
                            p1 = e.Location;
                        else
                        {
                            p2 = e.Location;
                            DrawLine((Bitmap)pictureBox1.Image, p1, p2, c);
                            pictureBox1.Refresh();
                            if (!DoublePoints)
                                p1 = new Point();
                            else
                                p1 = p2;
                            p2 = new Point();
                        }
                    }
                    break;
            }
            if(FillMethod > 0)
            {
            st.Stop();
            label1.Text = (st.ElapsedMilliseconds / 1000d).ToString();
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
        }
        #region FillMethoden
        private void FloodFill(Bitmap bmp, Point pt, Color replacementColor)
        {
            Stack<Point> pixels = new Stack<Point>();
            Color targetColor = bmp.GetPixel(pt.X, pt.Y);

            if (targetColor.R == replacementColor.R &&
                targetColor.G == replacementColor.G &&
                targetColor.B == replacementColor.B) return;

            pixels.Push(pt);

            while (pixels.Count > 0)
            {
                Point a = pixels.Pop();
                if (a.X < bmp.Width && a.X > 0 &&
                        a.Y < bmp.Height && a.Y > 0)//make sure we stay within bounds
                {
                    if (bmp.GetPixel(a.X, a.Y) == targetColor)
                    {
                        bmp.SetPixel(a.X, a.Y, replacementColor);
                        pixels.Push(new Point(a.X - 1, a.Y));
                        pixels.Push(new Point(a.X + 1, a.Y));
                        pixels.Push(new Point(a.X, a.Y - 1));
                        pixels.Push(new Point(a.X, a.Y + 1));
                        //pictureBox1.Refresh();
                    }
                }
            }
            pictureBox1.Refresh(); //refresh our main picture box
            return;
        }
        private void ScanFill(Bitmap bmp, Point pt, Color replacementColor)
        {
            Color targetColor = bmp.GetPixel(pt.X, pt.Y);

            if (targetColor.R == replacementColor.R &&
                targetColor.G == replacementColor.G &&
                targetColor.B == replacementColor.B) return;

            Stack<Point> pixels = new Stack<Point>();

            pixels.Push(pt);
            while (pixels.Count != 0)
            {
                Point temp = pixels.Pop();
                int y1 = temp.Y;
                while (y1 >= 0 && bmp.GetPixel(temp.X, y1) == targetColor)
                {
                    y1--;
                }
                y1++;
                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < bmp.Height && bmp.GetPixel(temp.X, y1) == targetColor)
                {
                    bmp.SetPixel(temp.X, y1, replacementColor);
                    //pictureBox1.Refresh();

                    if (!spanLeft && temp.X > 0 && bmp.GetPixel(temp.X - 1, y1) == targetColor)
                    {
                        pixels.Push(new Point(temp.X - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.X - 1 == 0 && bmp.GetPixel(temp.X - 1, y1) != targetColor)
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.X < bmp.Width - 1 && bmp.GetPixel(temp.X + 1, y1) == targetColor)
                    {
                        pixels.Push(new Point(temp.X + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.X < bmp.Width - 1 && bmp.GetPixel(temp.X + 1, y1) != targetColor)
                    {
                        spanRight = false;
                    }
                    y1++;
                }

            }
            pictureBox1.Refresh();

        }
        private void FloodFillQueue(Bitmap bmp, Point pt, Color replacementColor)
        {
            Queue<Point> pixels = new Queue<Point>();
            Color targetColor = bmp.GetPixel(pt.X, pt.Y);

            if (targetColor.R == replacementColor.R &&
                targetColor.G == replacementColor.G &&
                targetColor.B == replacementColor.B) return;

            pixels.Enqueue(pt);

            while (pixels.Count > 0)
            {
                Point a = pixels.Dequeue();
                if (a.X < bmp.Width && a.X > 0 &&
                        a.Y < bmp.Height && a.Y > 0)//make sure we stay within bounds
                {
                    if (bmp.GetPixel(a.X, a.Y) == targetColor)
                    {
                        bmp.SetPixel(a.X, a.Y, replacementColor);
                        pixels.Enqueue(new Point(a.X - 1, a.Y));
                        pixels.Enqueue(new Point(a.X + 1, a.Y));
                        pixels.Enqueue(new Point(a.X, a.Y - 1));
                        pixels.Enqueue(new Point(a.X, a.Y + 1));
                        //pictureBox1.Refresh();
                    }
                }
            }
            pictureBox1.Refresh(); //refresh our main picture box
            return;
        }
        private void ScanFillQueue(Bitmap bmp, Point pt, Color replacementColor)
        {
            Color targetColor = bmp.GetPixel(pt.X, pt.Y);

            if (targetColor.R == replacementColor.R &&
                targetColor.G == replacementColor.G &&
                targetColor.B == replacementColor.B) return;

            Queue<Point> pixels = new Queue<Point>();

            pixels.Enqueue(pt);
            while (pixels.Count != 0)
            {
                Point temp = pixels.Dequeue();
                int y1 = temp.Y;
                while (y1 >= 0 && bmp.GetPixel(temp.X, y1) == targetColor)
                {
                    y1--;
                }
                y1++;
                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < bmp.Height && bmp.GetPixel(temp.X, y1) == targetColor)
                {
                    bmp.SetPixel(temp.X, y1, replacementColor);
                    //pictureBox1.Refresh();

                    if (!spanLeft && temp.X > 0 && bmp.GetPixel(temp.X - 1, y1) == targetColor)
                    {
                        pixels.Enqueue(new Point(temp.X - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.X - 1 == 0 && bmp.GetPixel(temp.X - 1, y1) != targetColor)
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.X < bmp.Width - 1 && bmp.GetPixel(temp.X + 1, y1) == targetColor)
                    {
                        pixels.Enqueue(new Point(temp.X + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.X < bmp.Width - 1 && bmp.GetPixel(temp.X + 1, y1) != targetColor)
                    {
                        spanRight = false;
                    }
                    y1++;
                }

            }
            pictureBox1.Refresh();

        }
        #endregion

        private Bitmap DrawLine(Bitmap bitmap, Point p1, Point p2, Color c)
        {
            int dx = Math.Abs(p2.X - p1.X);
            int sx = p1.X < p2.X ? 1 : -1;
            int dy = Math.Abs(p2.Y - p1.Y);
            int sy = p1.Y < p2.Y ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2;
            int e2;
            for (; ; )
            {
                bitmap.SetPixel(p1.X, p1.Y, c);
                if (p1 == p2)
                    break;
                e2 = err;
                if (e2 > -dx)
                {
                    err -= dy;
                    p1.X += sx;
                }
                if (e2 < dy)
                {
                    err += dx;
                    p1.Y += sy;
                }
            }
            return bitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Reset();
            FillMethod = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Reset();
            FillMethod = 2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Reset();
            FillMethod = 3;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Reset();
            FillMethod = 4;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FillMethod = 0;
            ISDrawn = false;
            DoublePoints = false;
            p1 = new Point();
            p2 = new Point();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            c = colorDialog1.Color;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(!ISDrawn)
            {
                Reset();
                ISDrawn = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(!DoublePoints)
            {
                Reset();
                ISDrawn = true;
                DoublePoints = true;
            }

        }
        private void Reset()
        {
            p1 = new Point();
            p2 = new Point();
            ISDrawn = false;
            DoublePoints = false;
            FillMethod = 0;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            string f = Directory.GetCurrentDirectory();
            f = f.Substring(0, f.IndexOf("bin"));
            //string s = DateTime.Now.ToString().Replace(':','_').Replace('.', '_').Replace(' ', '_');
            string d = $"{DateTime.Now:dd-hh-mm-ss}";
            string p = $"{f}{d}.png";
            ((Bitmap)pictureBox1.Image).Save(p);
        }
    }
}
