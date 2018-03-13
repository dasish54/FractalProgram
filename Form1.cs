using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssignmentCode
{
    public partial class Form1 : Form
    {
        private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1 = 640, y1 = 480, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static float xy;
        Rectangle rec;
        public System.Drawing.Bitmap bitmap;
        //private Image picture;
        private Graphics g1;




        private HSB HSBcol = new HSB();
        // Saves image to C Drive when save button is pressed
        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
           

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
            ImageFormat format = ImageFormat.Png;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                switch (ext)
                {
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                }
                bitmap.Save(sfd.FileName, format);
            }
        }
        // Saves state to rows.txt file when save state button is pressed
        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string savexzoom = xzoom.ToString();
            string saveyzoom = yzoom.ToString();
            string saveystart = ystart.ToString();
            string savexstart = xstart.ToString();
            string[] rows = { savexzoom, saveyzoom, savexstart, saveystart };

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.FilterIndex = 2;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllLines(sfd.FileName, rows);
                }
            }
        }
        // Uses exit function when exit button is pressed
        private void exitApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        // Reads state from rows.txt then loadsthe state after load state button is pressed
        private void loadStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\";
            string line = "";
            var rows = new List<string>();
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                xzoom = System.Convert.ToDouble(rows[0]);
                yzoom = System.Convert.ToDouble(rows[1]);
                xstart = System.Convert.ToDouble(rows[2]);
                ystart = System.Convert.ToDouble(rows[3]);

            }
            


           

            mandelbrot();
        }

        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // Instantiates form and initialises values
        public Form1()
        {
            this.DoubleBuffered = true;
            init();
            start();
            InitializeComponent();
        }




        public void init() // all instances will be prepared
        {
            //HSBcol = new HSB();
            this.Width = 640;
            this.Height = 480;

            //finished = false;

            //c1 = Cursors.Default;
            //c2 = Cursors.IBeam;
            xy = (float)x1 / (float)y1;
            //picture = createImage(x1, y1);
            //g1 = picture.getGraphics();
            bitmap = new Bitmap(x1, y1);
            g1 = Graphics.FromImage(bitmap);
            // finished = true;
        }



        public void start()
        {
            //action = false;
            //rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }




        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;
            Color color;
            Pen fractPen = new Pen(Color.Black);
            // action = false;
            //  setCursor(c1);
            // showStatus("Mandelbrot-Set will be produced - please wait...");
            for (x = 0; x < x1; x += 2)
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                    if (h != alt)
                    {
                        b = 1.0f - h * h;
                        // brightnes
                        ///djm added
                        ///HSBcol.fromHSB(h,0.8f,b); //convert hsb to rgb then make a Java Color
                        color = HSB.FromHSB(new HSB(h * 255, 0.8f * 255, b * 255));
                        ///g1.setColor(col);
                        //djm end
                        //djm added to convert to RGB from HSB

                        // g1.setColor(Color.getHSBColor(h, 0.8f, b));
                        //djm test
                        // Color col = Color.getHSBColor(h, 0.8f, b);
                        // int red = col.getRed();
                        // int green = col.getGreen();
                        // int blue = col.getBlue();
                        //djm 
                        fractPen = new Pen(color);
                        alt = h;
                    }

                    g1.DrawLine(fractPen, x, y, x + 1, y);
                }
            //showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
            //setCursor(c2);
            //action = true;
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2.0 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }

        private void initvalues() // reset start values
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;
        }

        //  public void mousePressed(MouseEvent e)
        //{

        //}
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g1 = e.Graphics;
            g1.DrawImage(bitmap, 0, 0, x1, y1);
            using (Pen pen = new Pen(Color.White, 1))
            {
                e.Graphics.DrawRectangle(pen, rec);

            }
            Invalidate();

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            int z, w;


            if (e.Button == MouseButtons.Left)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
                this.Invalidate();
            }
        }


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {

                xs = e.X;
                ys = e.Y;
                this.Invalidate();
            }
        }


        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                xe = e.X;
                ye = e.Y;
                if (xs < xe)
                {
                    if (ys < ye) rec = new Rectangle(xs, ys, (xe - xs), (ye - ys));
                    else rec = new Rectangle(xs, ye, (xe - xs), (ys - ye));
                }
                else
                {
                    if (ys < ye) rec = new Rectangle(xe, ys, (xs - xe), (ye - ys));
                    else rec = new Rectangle(xe, ye, (xs - xe), (ys - ye));
                }


                this.Invalidate();
            }


        }


        //Don't think this is needed due to APPLET
        // public String getAppletInfo()
        //{
        //   return "fractal.class - Mandelbrot Set a Java Applet by Eckhard Roessel 2000-2001";
        //}
    }

    
        public struct HSB
        {
            float h;
            float s;
            float b;
            int a;
            public HSB(float h, float s, float b)
            {
                this.a = 0xff;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }

            public HSB(int a, float h, float s, float b)
            {
                this.a = a;
                this.h = Math.Min(Math.Max(h, 0), 255);
                this.s = Math.Min(Math.Max(s, 0), 255);
                this.b = Math.Min(Math.Max(b, 0), 255);
            }
            public float H { get { return h; } }
            public float S { get { return s; } }
            public float B { get { return b; } }
            public int A { get { return a; } }
            public Color Color { get { return FromHSB(this); } }
            public static Color FromHSB(HSB hsbColor)
            {
                float red = hsbColor.b;
                float green = hsbColor.b;
                float blue = hsbColor.b;
                if (hsbColor.s != 0)
                {
                    float max = hsbColor.b;
                    float dif = hsbColor.b * hsbColor.s / 255f;
                    float min = hsbColor.b - dif;
                    float h2 = hsbColor.h * 360f / 255f;

                    if (h2 < 60f)
                    {
                        red = max;
                        green = h2 * dif / 60f + min;
                        blue = min;
                    }
                    else if (h2 < 120f)
                    {
                        red = -(h2 - 120f) * dif / 60f + min;
                        green = max;
                        blue = min;
                    }
                    else if (h2 < 180f)
                    {
                        red = min;
                        green = max;
                        blue = (h2 - 120f) * dif / 60f + min;
                    }
                    else if (h2 < 240f)
                    {
                        red = min;
                        green = -(h2 - 240f) * dif / 60f + min;
                        blue = max;
                    }
                    else if (h2 < 300f)
                    {
                        red = (h2 - 240f) * dif / 60f + min;
                        green = min;
                        blue = max;
                    }
                    else if (h2 <= 360f)
                    {
                        red = max;
                        green = min;
                        blue = -(h2 - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                    }
                }
                return Color.FromArgb(hsbColor.a, (int)Math.Round(Math.Min(Math.Max(red, 0), 255)), (int)Math.Round(Math.Min(Math.Max(green, 0), 255)), (int)Math.Round(Math.Min(Math.Max(blue, 0), 255)));
            }

        }
    }

