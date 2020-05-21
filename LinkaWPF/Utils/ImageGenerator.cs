using System;
using System.Drawing;

namespace LinkaWPF.Utils
{
    public class ImageGenerator
    {
        private readonly Color BACK_COLOR = Color.White;
        private readonly Color TEXT_COLOR = Color.Black;
        private readonly Font FONT = new Font(SystemFonts.DefaultFont.FontFamily,48f, FontStyle.Bold);
        private readonly int PADDING = 5;

        public void GenerateImage(String text, String file)
        {
           PadImage(DrawText(text)).Save(file);
        }
        private Image PadImage(Image originalImage)
        {
            int largestDimension = Math.Max(originalImage.Height, originalImage.Width);
            Size squareSize = new Size(largestDimension, largestDimension);
            Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height);
            using (Graphics graphics = Graphics.FromImage(squareImage))
            {
                graphics.Clear(BACK_COLOR);
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                graphics.DrawImage(originalImage, (squareSize.Width / 2) - (originalImage.Width / 2), (squareSize.Height / 2) - (originalImage.Height / 2), originalImage.Width, originalImage.Height);
            }
            return squareImage;
        }
        private Image DrawText(String text)
        {
            
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);
            
            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, FONT);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width + PADDING, (int)textSize.Height+PADDING);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(BACK_COLOR);

            //create a brush for the text
            Brush textBrush = new SolidBrush(TEXT_COLOR);

            drawing.DrawString(text, FONT, textBrush, PADDING/2f, PADDING/2f);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;

        }
    }
}
