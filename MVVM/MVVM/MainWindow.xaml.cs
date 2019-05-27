using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MVVM
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

        private void SldAge_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblAge.Content = sldAge.Value;
        }

        private void BtnAddImg_Click(object sender, RoutedEventArgs e)
        {
            string imageFolderSave = "Image";
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) " +
                "| *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dlg.ShowDialog() == true)
            {
                img.Source = new BitmapImage(new Uri(dlg.FileName));
                try
                {
                    var filePath = dlg.FileName;
                    var image = System.Drawing.Image.FromFile(dlg.FileName);
                    string   ImgName = Guid.NewGuid().ToString() + ".jpg";
                    File.Copy(filePath, System.IO.Path.Combine(imageFolderSave, ImgName));
                    if (!Directory.Exists(imageFolderSave))
                    {
                        Directory.CreateDirectory(imageFolderSave);
                    }
                    var bmpOrigin = new System.Drawing.Bitmap(image);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Щось пішло не так {ex.Message}");
                }
            }
        }
    }
}
