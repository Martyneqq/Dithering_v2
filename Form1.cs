using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Dithering
{
    public partial class Form1 : Form
    {
        Bitmap original, remake;
        Color originalColor, newColor;
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Reset();
        }
        private void Reset()
        {
            if (original != null)
            {
                pictureBox1.Image = new Bitmap(original);
            }
            else
            {
                MessageBox.Show("Load an image first");
            }
        }
        private void Free()
        {
            if (original != null) // get rid of an old image
            {
                original.Dispose();
                original = null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (original == null)
                {
                    throw new ArgumentNullException(paramName: nameof(remake), message: "error");
                }
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
                if (save.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image.Save(save.FileName);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Load a picture before saving");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnfd = new OpenFileDialog();
            opnfd.Filter = "Image Files|*.jpg;*.jpeg;*.png;";

            Free();
            if (opnfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(opnfd.FileName);
                original = (Bitmap)pictureBox1.Image;
            }
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                RandomDithering();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                ClusteredDotDithering();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                OrderedDithering();
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                ErrorDiffusion(7.0f / 16.0f, 3.0f / 16.0f, 5.0f / 16.0f, 1.0f / 16.0f);
            }
        }
        private Color MakeGray(Color originalColor)
        {
            int gray = (byte)(originalColor.R / 3 + originalColor.G / 3 + originalColor.B / 3);
            originalColor = Color.FromArgb(originalColor.A, gray, gray, gray);
            return originalColor;
        }
        private Color MakeBlack(Color originalColor)
        {
            int threshold = trackBar1.Value;
            int gray = (byte)(originalColor.R / 3 + originalColor.G / 3 + originalColor.B / 3);
            if (gray < threshold) // Threshold white black
            {
                originalColor = Color.FromArgb(originalColor.A, 0, 0, 0);

            }
            else
            {
                originalColor = Color.FromArgb(originalColor.A, 255, 255, 255);
            }
            return originalColor;
        } // Threshold 
        private Color MakeBlack(Color originalColor, double rnd) // Random
        {
            int gray = (byte)(originalColor.R / 3 + originalColor.G / 3 + originalColor.B / 3);
            if (gray < rnd) // Threshold random values
            {
                originalColor = Color.FromArgb(originalColor.A, 0, 0, 0);

            }
            else
            {
                originalColor = Color.FromArgb(originalColor.A, 255, 255, 255);
            }
            return originalColor;
        }
        private Color MakeBlack(Color originalColor, int value) // Error
        {
            int gray = (byte)(originalColor.R / 3 + originalColor.G / 3 + originalColor.B / 3);
            if (gray < value) // Threshold value
            {
                originalColor = Color.FromArgb(originalColor.A, 0, 0, 0);
            }
            else
            {
                originalColor = Color.FromArgb(originalColor.A, 255, 255, 255);
            }
            return originalColor;
        }
        private void ThresholdImage(Bitmap original, Bitmap remake)
        {
            try
            {
                if (original == null)
                {
                    throw new ArgumentNullException(paramName: nameof(remake), message: "error");
                }
                remake = new Bitmap(original);
                // Load image
                for (int x = 0; x < original.Width; x++)
                {
                    for (int y = 0; y < original.Height; y++)
                    {
                        originalColor = original.GetPixel(x, y);

                        //newColor = Color.FromArgb(originalColor.A, gray, gray, gray); // Image is gray until now
                        newColor = MakeGray(originalColor);

                        remake.SetPixel(x, y, newColor);
                    }
                }

                // Display
                pictureBox1.Image = remake;
                remake = (Bitmap)pictureBox1.Image;
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error!");
            }
        }
        private void RandomDithering()
        {
            Random rng = new Random();
            try
            {
                if (original == null)
                {
                    throw new ArgumentNullException(paramName: nameof(remake), message: "error");
                }
                remake = new Bitmap(original);
                // Loop through the images pixels to reset color.
                for (int x = 0; x < original.Width; x++)
                {
                    for (int y = 0; y < original.Height; y++)
                    {
                        double rnd = rng.Next(0, 255);
                        originalColor = original.GetPixel(x, y);
                        newColor = MakeBlack(originalColor, rnd);
                        if (checkBox1.Checked)
                        {
                            float newIntensity = EnhancePixel(x, y, remake, newColor);
                            newColor = Color.FromArgb((byte)newIntensity, (byte)newIntensity, (byte)newIntensity);
                        }
                        remake.SetPixel(x, y, newColor);
                    }
                }

                pictureBox1.Image = remake;
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error in RandomDithering!");
            }
        }
        private void ClusteredDotDithering() // non-functional
        {
            int[,] k = {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 255, 0, 0 },
            { 0, 0, 0, 0, 0, 255, 255, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 255, 0, 0, 0, 0, 0, 0 },
            { 0, 255, 255, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 }};
            int n = k.GetLength(0);

            try
            {
                if (original == null)
                {
                    throw new ArgumentNullException(paramName: nameof(remake), message: "error");
                }
                remake = new Bitmap(original);
                // Loop through the images pixels to reset color.
                for (int i = 0; i < 63; i++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        for (int y = 0; y < original.Height; y++)
                        {
                            //k[x, y];
                            int gray = (byte)(originalColor.R / 3 + originalColor.G / 3 + originalColor.B / 3);
                            originalColor = original.GetPixel(x, y);
                            Color grayColor = MakeGray(originalColor);
                            newColor = MakeBlack(originalColor);
                            original.SetPixel(x, y, newColor);

                            if (gray < 128)
                            {
                                newColor = Color.FromArgb(originalColor.A, 0, 0, 0);
                            }
                            else
                            {
                                newColor = Color.FromArgb(originalColor.A, 255, 255, 255);
                            }
                            /*
                            float errorR = grayColor.R - newColor.R;
                            float errorG = grayColor.G - newColor.G;
                            float errorB = grayColor.B - newColor.B;

                            int w = 0;
                            */

                            remake.SetPixel(x, y, newColor);
                        }
                    }
                }

                pictureBox1.Image = remake;
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error in OrderedDithering!");
            }
        }
        private void OrderedDithering()
        {
            int threshold = trackBar1.Value;
            //Debug.WriteLine("Value of threshold: " + threshold);
            int[,] D = { { 43, 32, 64 }, { 255, 0, 85 }, { 51, 128, 37 } };

            int n = D.GetLength(0);

            try
            {
                if (original == null)
                {
                    throw new ArgumentNullException(paramName: nameof(remake), message: "error");
                }
                remake = new Bitmap(original);
                // Loop through the images pixels to reset color.
                for (int x = 0; x < original.Width; x++)
                {
                    for (int y = 0; y < original.Height; y++)
                    {
                        originalColor = original.GetPixel(x, y);
                        Color newColor = originalColor;
                        original.SetPixel(x, y, newColor);

                        float I = newColor.GetBrightness() * 255;

                        //Debug.Write(D[x % n, y % n] + " ");
                        if (D[x % n, y % n] + threshold < (threshold * 2 - I) * (Math.Pow(n, 2)))
                        {
                            newColor = Color.FromArgb(originalColor.A, 0, 0, 0);
                        }
                        else
                        {
                            newColor = Color.FromArgb(originalColor.A, 255, 255, 255);
                        }

                        remake.SetPixel(x, y, newColor);
                    }
                    //Debug.WriteLine("\n");
                }
                pictureBox1.Image = remake;
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error in OrderedDithering!");
            }
        }
        private void ErrorDiffusion(float alpha, float beta, float gamma, float delta)
        {
            try
            {
                if (original == null)
                {
                    throw new ArgumentNullException(paramName: nameof(remake), message: "error");
                }
                remake = new Bitmap(original);
                for (int x = 1; x < original.Width - 1; x++)
                {
                    for (int y = 1; y < original.Height - 1; y++)
                    {
                        originalColor = original.GetPixel(x, y);
                        Color grayColor = MakeGray(originalColor);
                        newColor = MakeBlack(grayColor, 128);
                        original.SetPixel(x, y, newColor);

                        float errorR = grayColor.R - newColor.R;
                        float errorG = grayColor.G - newColor.G;
                        float errorB = grayColor.B - newColor.B;

                        newColor = FindErrorPixel(grayColor, x, y + 1, errorR, errorG, errorB, alpha);
                        remake.SetPixel(x, y + 1, newColor);

                        newColor = FindErrorPixel(grayColor, x + 1, y - 1, errorR, errorG, errorB, beta);
                        remake.SetPixel(x + 1, y - 1, newColor);

                        newColor = FindErrorPixel(grayColor, x + 1, y, errorR, errorG, errorB, gamma);
                        remake.SetPixel(x + 1, y, newColor);

                        newColor = FindErrorPixel(grayColor, x + 1, y + 1, errorR, errorG, errorB, delta);
                        remake.SetPixel(x + 1, y + 1, newColor);
                    }
                }
                // Set the PictureBox to display the image.
                pictureBox1.Image = remake;
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error in ErrorDittering!");
            }
        }
        private Color FindErrorPixel(Color grayColor, int posX, int posY, float errorR, float errorG, float errorB, float offset)
        {
            newColor = original.GetPixel(posX, posY);

            float r0 = newColor.R + errorR * offset;
            float g0 = newColor.G + errorG * offset;
            float b0 = newColor.B + errorB * offset;
            return newColor = Color.FromArgb(grayColor.A, (byte)r0, (byte)g0, (byte)b0);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (comboBox1.SelectedIndex == 0)
            {
                RandomDithering(original, remake);
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                ClusteredDotDithering();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                OrderedDithering();
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                ErrorDiffusion(original, remake, 7.0f / 16.0f, 3.0f / 16.0f, 5.0f / 16.0f, 1.0f / 16.0f);
            }*/
        }
        private float AvarageIntensity(int x, int y, Bitmap bitmap, Color color)
        {
            int[] offsetX = { 0, -1, 0, 1, 1, 1, 0, -1, -1 };
            int[] offsetY = { 0, 1, 1, 1, 0, -1, -1, -1, 0 };
            float result = 0;

            for (int i = 0; i < 8; i++)
            {
                int ox = x + offsetX[i];
                int oy = y + offsetY[i];
                if (ox > 0 && ox < bitmap.Width && oy > 0 && oy < bitmap.Height)
                {
                    color = bitmap.GetPixel(ox, oy);
                    result += color.GetBrightness() * 255;
                }
            }
            result /= 9;
            return result;
        }
        private float EnhancePixel(int x, int y, Bitmap bitmap, Color color)
        {
            int a = 128;
            color = bitmap.GetPixel(x, y);
            float intensity = color.GetBrightness() * 255;
            float avarageIntensity = AvarageIntensity(x, y, bitmap, color);

            float enhancedIntensity = ((a * avarageIntensity) - (intensity)) / (255 - a);

            return enhancedIntensity;
        }
    }
}
