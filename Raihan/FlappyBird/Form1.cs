using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlappyBird.Properties;

namespace FlappyBird
{
    public partial class MainForm : Form
    {
        List<int> Stopper1 = new List<int>();
        List<int> Stopper2 = new List<int>();
        int StopperWidth = 50;
        int StopperDifferentY = 140;
        int StopperDifferentX = 190;
        bool start = true;
        bool running;
        int step = 5;
        int Originalx, Originaly;
        bool ResetStopper = false;
        int points;
        bool inDeadArea = false;
        int score;
        int scoredifferent;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Originalx = pictureBox1.Location.X;
            Originaly = pictureBox1.Location.Y;
            if (!File.Exists("Score.ini"))
            {
                File.Create("Score.ini").Dispose();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void Bird_Die()
        {
            running = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            button1.Visible = true;
            button1.Enabled = true;
            ReadAndShowScore();
            points = 0;
            pictureBox1.Location = new Point(Originalx, Originaly);
            ResetStopper = true;
            Stopper1.Clear();
        }

        private void ReadAndShowScore()
        {
            using (StreamReader reader = new StreamReader("Score.ini"))
            {
                score = int.Parse(reader.ReadToEnd());
                reader.Close();
                if (int.Parse(label1.Text) == 0 | int.Parse(label1.Text) > 0)
                {
                    scoredifferent = score - int.Parse(label1.Text) + 1;
                }
                if (score < int.Parse(label1.Text))
                {
                    MessageBox.Show(string.Format("Congratulations, you made a higher score than {0}. The new score is {1}", score, label1.Text), "Flappy Bird", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    using (StreamWriter writer = new StreamWriter("Score.ini"))
                    {
                        writer.Write(label1.Text);
                        writer.Close();
                    }
                }
                if (score > int.Parse(label1.Text))
                {
                    MessageBox.Show(string.Format(" You need {0} to overcome max score {1}", scoredifferent, score), "Flappy Bird", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (score == int.Parse(label1.Text))
                {
                    MessageBox.Show(string.Format("You did exactly {0} (score max). Try to overtake this time.", score), "Flappy Bird", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void PlayGame()
        {
            ResetStopper = false;
            timer1.Enabled = true;
            timer2.Enabled = true;
            timer3.Enabled = true;
            Random random = new Random();
            int num = random.Next(40, this.Height - this.StopperDifferentY);
            int num1 = num + this.StopperDifferentY;
            Stopper1.Clear();
            Stopper1.Add(this.Width);
            Stopper1.Add(num);
            Stopper1.Add(this.Width);
            Stopper1.Add(num1);

            num = random.Next(40, (this.Height - StopperDifferentY));
            num1 = num + StopperDifferentY;
            Stopper2.Clear();
            Stopper2.Add(this.Width + StopperDifferentX);
            Stopper2.Add(num);
            Stopper2.Add(this.Width + StopperDifferentX);
            Stopper2.Add(num1);

            button1.Visible = false;
            button1.Enabled = false;
            running = true;
            Focus();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Stopper1[0] + StopperWidth <= 0 | start == true)
            {
                Random rnd = new Random();
                int px = this.Width;
                int py = rnd.Next(40, (this.Height - StopperDifferentY));
                var p2x = px;
                var p2y = py + StopperDifferentY;
                Stopper1.Clear();
                Stopper1.Add(px);
                Stopper1.Add(py);
                Stopper1.Add(p2x);
                Stopper1.Add(p2y);
            }
            else
            {
                Stopper1[0] = Stopper1[0] - 2;
                Stopper1[2] = Stopper1[2] - 2;
            }
            if (Stopper2[0] + StopperWidth <= 0)
            {
                Random rnd = new Random();
                int px = this.Width;
                int py = rnd.Next(40, (this.Height - StopperDifferentY));
                var p2x = px;
                var p2y = py + StopperDifferentY;
                int[] p1 = { px, py, p2x, p2y };
                Stopper2.Clear();
                Stopper2.Add(px);
                Stopper2.Add(py);
                Stopper2.Add(p2x);
                Stopper2.Add(p2y);
            }
            else
            {
                Stopper2[0] = Stopper2[0] - 2;
                Stopper2[2] = Stopper2[2] - 2;
            }
            if (start == true)
            {
                start = false;
            }
        }

        private void PointChecking()
        {
            Rectangle rec = pictureBox1.Bounds;
            Rectangle rec1 = new Rectangle(Stopper1[2] + 20, Stopper1[3] - StopperDifferentY, 15, StopperDifferentY);
            Rectangle rec2 = new Rectangle(Stopper2[2] + 20, Stopper2[3] - StopperDifferentY, 15, StopperDifferentY);
            Rectangle intersect1 = Rectangle.Intersect(rec, rec1);
            Rectangle intersect2 = Rectangle.Intersect(rec, rec2);
            if (!ResetStopper | start)
            {
                if (intersect1 != Rectangle.Empty | intersect2 != Rectangle.Empty)
                {
                    if (!inDeadArea)
                    {
                        points++;
                        SoundPlayer sp = new SoundPlayer(FlappyBird.Properties.Resources.point);
                        sp.Play();
                        inDeadArea = true;
                    }
                }
                else
                {
                    inDeadArea = false;
                }
            }
        }

        private void CollisionChecking()
        {
            Rectangle rec = pictureBox1.Bounds;
            Rectangle rec1 = new Rectangle(Stopper1[0], 0, StopperWidth, Stopper1[1]);
            Rectangle rec2 = new Rectangle(Stopper1[2], Stopper1[3], StopperWidth, this.Height - Stopper1[3]);
            Rectangle rec3 = new Rectangle(Stopper2[0], 0, StopperWidth, Stopper2[1]);
            Rectangle rec4 = new Rectangle(Stopper2[2], Stopper2[3], StopperWidth, this.Height - Stopper2[3]);
            Rectangle intersect1 = Rectangle.Intersect(rec, rec1);
            Rectangle intersect2 = Rectangle.Intersect(rec, rec2);
            Rectangle intersect3 = Rectangle.Intersect(rec, rec3);
            Rectangle intersect4 = Rectangle.Intersect(rec, rec4);
            if (!ResetStopper | start)
            {
                if (intersect1 != Rectangle.Empty | intersect2 != Rectangle.Empty | intersect3 != Rectangle.Empty | intersect4 != Rectangle.Empty)
                {
                    SoundPlayer sp = new SoundPlayer(FlappyBird.Properties.Resources.collision);
                    sp.Play();
                    Bird_Die();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PlayGame();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (!ResetStopper && Stopper1.Any() && Stopper2.Any())
            {

                e.Graphics.FillRectangle(Brushes.LimeGreen, new Rectangle(Stopper1[0], 0, StopperWidth, Stopper1[1]));
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(Stopper1[0] - 8, Stopper1[3] - StopperDifferentY, 65, 15));


                e.Graphics.FillRectangle(Brushes.LimeGreen, new Rectangle(Stopper1[2], Stopper1[3], StopperWidth, this.Height - Stopper1[3]));
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(Stopper1[2] - 8, Stopper1[3], 65, 15));


                e.Graphics.FillRectangle(Brushes.LimeGreen, new Rectangle(Stopper2[0], 0, StopperWidth, Stopper2[1]));
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(Stopper2[0] - 8, Stopper2[3] - StopperDifferentY, 65, 15));


                e.Graphics.FillRectangle(Brushes.LimeGreen, new Rectangle(Stopper2[2], Stopper2[3], StopperWidth, this.Height - Stopper2[3]));
                e.Graphics.FillRectangle(Brushes.DarkGreen, new Rectangle(Stopper2[2] - 8, Stopper2[3], 65, 15));

            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    step = -5;
                    pictureBox1.Image = Resources.bird_straight;
                    break;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            pictureBox1.Location = new Point(pictureBox1.Location.X, pictureBox1.Location.Y + step);
            if (pictureBox1.Location.Y < 0)
            {
                pictureBox1.Location = new Point(pictureBox1.Location.X, 0);
            }
            if (pictureBox1.Location.Y + pictureBox1.Height > this.ClientSize.Height)
            {
                pictureBox1.Location = new Point(pictureBox1.Location.X, this.ClientSize.Height - pictureBox1.Height);
            }
            CollisionChecking();
            if (running)
            {
                PointChecking();
            }
            label1.Text = Convert.ToString(points);
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    step = 5;
                    pictureBox1.Image = Resources.bird_down;
                    break;
            }
        }
    }
}
