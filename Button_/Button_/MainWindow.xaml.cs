using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Button_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double screeHeight = SystemParameters.FullPrimaryScreenHeight;
            double screeWidth = SystemParameters.FullPrimaryScreenWidth;
            MaxHeight = screeHeight;
            MaxWidth = screeWidth;
            OpenFileDialog dlg = new OpenFileDialog();
            btnImage.MaxHeight = Height;
            btnImage.MaxWidth = Width;

            try
            {
                dlg.Title = "Select a picture";
                if (dlg.ShowDialog() == true)
                {
                    img.Source = new BitmapImage(new Uri(dlg.FileName));
                    btnImage.Height = 200;
                    btnImage.Width = 300;

                }
            }
            catch
            {


            }
        }
        bool c = false;
        private void BtnImage_MouseEnter(object sender, MouseEventArgs e)
        {

            c = true;
        }

        private void BtnImage_MouseLeave(object sender, MouseEventArgs e)
        {
            c = false;
        }

    

        private void BtnImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (e.Delta > 0)
                {
                    btnImage.Height += 50;
                    btnImage.Width += 50;
                    img.Height += 50;
                    img.Width += 50;
                }

                else if (e.Delta < 0)
                {
                    btnImage.Height -= 50;
                    btnImage.Width -= 50;
                    img.Height -= 50;
                    img.Width -= 50;

                }

            }
            catch { }

        }

   



        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try {
                btnImage.MaxHeight = Height-50;
                btnImage.MaxWidth = Width-50;
                MinHeight = btnImage.Height;
                MinWidth = btnImage.Width;

            }
            catch
            {
            }
            }

        
    }
}
