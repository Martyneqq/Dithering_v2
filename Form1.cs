using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            ThresholdImage(original, remake);
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
                ErrorDiffusion(original, remake, 7.0f / 16.0f, 3.0f/16.0f, 5.0f/16.0f, 1.0f/16.0f);
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
                else
                {
                    // Load image
                    for (int x = 0; x < original.Width; x++)
                    {
                        for (int y = 0; y < original.Height; y++)
                        {
                            remake = original;
                            originalColor = original.GetPixel(x, y);

                            //newColor = Color.FromArgb(originalColor.A, gray, gray, gray); // Image is gray until now
                            newColor = MakeGray(originalColor);

                            remake.SetPixel(x, y, newColor);
                        }
                    }

                    // Display
                    pictureBox1.Image = remake;
                    remake = (Bitmap)pictureBox1.Image;
                    // How to save the image?
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error!");
            }
        }
        private void RandomDithering(Bitmap original, Bitmap remake)
        {
            Random rng = new Random();
            try
            {
                if (original==null)
                {
                    throw new ArgumentNullException(paramName: nameof(remake), message: "error");
                }
                // Loop through the images pixels to reset color.
                for (int x = 0; x < original.Width; x++)
                {
                    for (int y = 0; y < original.Height; y++)
                    {
                        remake = original;
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
                // Loop through the images pixels to reset color.
                for (int i = 0; i < 63; i++)
                {
                    for (int x = 0; x < original.Width; x++)
                    {
                        for (int y = 0; y < original.Height; y++)
                        {
                            //k[x, y];
                            remake = original;
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
            int[,] D = { { 43, 32, 64 },{ 255, 0, 85 },{ 51, 128, 37 } };

            int n = D.GetLength(0);

            try
            {
                if (original == null)
                {
                    throw new ArgumentNullException(paramName: nameof(remake), message: "error");
                }
                // Loop through the images pixels to reset color.
                for (int x = 0; x < original.Width; x++)
                {
                    for (int y = 0; y < original.Height; y++)
                    {
                        remake = original;
                        originalColor = original.GetPixel(x, y);
                        Color newColor = originalColor;
                        original.SetPixel(x, y, newColor);

                        float I = newColor.GetBrightness()*255;

                        if (D[x%n, y%n] + threshold < (threshold*2 - I)*(n*n))
                        {
                            newColor = Color.FromArgb(originalColor.A, 0, 0, 0);
                        }
                        else
                        {
                            newColor = Color.FromArgb(originalColor.A, 255, 255, 255);
                        }

                        remake.SetPixel(x, y, newColor);
                    }
                }

                pictureBox1.Image = remake;
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error in OrderedDithering!");
            }
        }
        private void ErrorDiffusion(Bitmap original, Bitmap remake, float alpha, float beta, float gamma, float delta)
        {
            try
            {
                if (original == null)
                {
                    throw new ArgumentNullException(paramName: nameof(remake), message: "error");
                }
                else
                {
                    for (int x = 1; x < original.Width-1; x++)
                    {
                        for (int y = 1; y < original.Height-1; y++)
                        {
                            remake = original;
                            originalColor = original.GetPixel(x, y);
                            Color grayColor = MakeGray(originalColor);
                            newColor = MakeBlack(grayColor, 128);
                            original.SetPixel(x, y, newColor);

                            float errorR = grayColor.R - newColor.R;
                            float errorG = grayColor.G - newColor.G;
                            float errorB = grayColor.B - newColor.B;

                            newColor = original.GetPixel(x, y + 1);
                            float r0 = newColor.R + errorR * alpha;
                            float g0 = newColor.G + errorG * alpha;
                            float b0 = newColor.B + errorB * alpha;
                            newColor = Color.FromArgb(grayColor.A, (byte)r0, (byte)g0, (byte)b0);
                            remake.SetPixel(x, y + 1, newColor);

                            newColor = original.GetPixel(x + 1, y - 1);
                            float r1 = newColor.R + errorR * beta;
                            float g1 = newColor.G + errorG * beta;
                            float b1 = newColor.B + errorB * beta;
                            newColor = Color.FromArgb(grayColor.A, (byte)r1, (byte)g1, (byte)b1);
                            remake.SetPixel(x+1, y - 1, newColor);

                            newColor = original.GetPixel(x + 1, y);
                            float r2 = newColor.R + errorR * gamma;
                            float g2 = newColor.G + errorG * gamma;
                            float b2 = newColor.B + errorB * gamma;
                            newColor = Color.FromArgb(grayColor.A, (byte)r2, (byte)g2, (byte)b2);
                            remake.SetPixel(x + 1, y, newColor);

                            newColor = original.GetPixel(x + 1, y + 1);
                            float r3 = newColor.R + errorR * delta;
                            float g3 = newColor.G + errorG * delta;
                            float b3 = newColor.B + errorB * delta;
                            newColor = Color.FromArgb(grayColor.A, (byte)r3, (byte)g3, (byte)b3);
                            remake.SetPixel(x + 1, y + 1, newColor);
                        }
                    }
                    // Set the PictureBox to display the image.
                    pictureBox1.Image = remake;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error in ErrorDittering!");
            }
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

            float enhancedIntensity = ((a * avarageIntensity) - (intensity))/(255-a);
            
            return enhancedIntensity;
        }
    }
}
