using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Screen
{
    /// <summary>
    /// PictrueWindows.xaml 的交互逻辑
    /// </summary>
    public partial class PictrueWindow : Window
    {
        private double width;
        private double height;
        Bitmap bitmap;
        BitmapSource bitmapSource;
        public PictrueWindow()
        {
            InitializeComponent();
            GetScreenSize();
            bitmap = new Bitmap(Convert.ToInt32(width), Convert.ToInt32(height));
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(Convert.ToInt32(width), Convert.ToInt32(height)));
            }
            bitmapSource = ChangeBitmapToBitmapSource(bitmap);
            ImageBrush imageBrush = new ImageBrush(bitmapSource);
            back.Background = imageBrush;

        }
        public PictrueWindow(BitmapImage bitmapImage) : this()
        {

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        private void GetScreenSize()
        {
            width = SystemParameters.PrimaryScreenWidth;
            height = SystemParameters.PrimaryScreenHeight;
        }
        private BitmapSource ChangeBitmapToBitmapSource(Bitmap bmp)
        {
            BitmapSource returnSource;
            try
            {
                returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                returnSource = null;
            }
            return returnSource;
        }
        BitmapSource outImage;
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (outImage != null)
                Clipboard.SetImage(outImage);
            this.Close();
        }
        System.Windows.Point frist;
        System.Windows.Point last;
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
        bool isDown = false;
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            frist = e.GetPosition(this);
            isDown = true;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                selectRectangle.Visibility = Visibility.Visible;
                System.Windows.Point point = e.GetPosition(this);
                double left = Math.Min(frist.X, point.X);
                double top = Math.Min(frist.Y, point.Y);
                double w = Math.Abs(frist.X - point.X);
                double h = Math.Abs(frist.Y - point.Y);

                selectRectangle.Margin = new Thickness(left, top, 0, 0);
                selectRectangle.Width = w;
                selectRectangle.Height = h;
                selectRectangle.HorizontalAlignment = HorizontalAlignment.Left;
                selectRectangle.VerticalAlignment = VerticalAlignment.Top;

                BitmapSource bs = bitmapSource.Clone();
                int stride = Convert.ToInt32(bs.Format.BitsPerPixel * w / 8);
                byte[] data = new byte[(int)h * stride * 4];
                try
                {
                    Int32Rect rect = new Int32Rect((int)left, (int)top, (int)w, (int)h);
                    bs.CopyPixels(rect, data, stride, 0);
                    outImage = BitmapSource.Create((int)w, (int)h, 0, 0, PixelFormats.Bgr32, null, data, stride);
                    selectRectangle.Fill = new ImageBrush(outImage);
                }
                catch (Exception)
                {

                 
                }
            
            }

        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDown = false;
        }
    }
}
