using Image.Help;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace Image
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

        private void BtnSelectedImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) " +
                "| *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var imageFolderSave = "Image";
                    var filePath = dlg.FileName;
                    var image = System.Drawing.Image.FromFile(dlg.FileName);
                    if (!Directory.Exists(imageFolderSave))
                    {
                        Directory.CreateDirectory(imageFolderSave);
                    }
                   
                    var bmpOrigin = new System.Drawing.Bitmap(image);
                    string []imageName = {"50_"+ Guid.NewGuid().ToString() + ".jpg" ,
                    "100_"+ Guid.NewGuid().ToString() + ".jpg",
                     "300_"+Guid.NewGuid().ToString() + ".jpg",
                    "600_"+ Guid.NewGuid().ToString() + ".jpg",
                    "1280_"+ Guid.NewGuid().ToString() + ".jpg"};

                    Bitmap [] imageSave = { ImageWorker.CreateImage(bmpOrigin, 50, 50),
                    ImageWorker.CreateImage(bmpOrigin, 100, 100),
                    ImageWorker.CreateImage(bmpOrigin, 300, 300),
                    ImageWorker.CreateImage(bmpOrigin, 600, 600),
                    ImageWorker.CreateImage(bmpOrigin, 1280, 1280)};
                    if (imageSave == null)
                        throw new Exception("Помилка");
                    for (int i = 0; i < imageName.Count(); i++)
                    {
                        var imageSaveEnd = System.IO.Path.Combine(imageFolderSave, imageName[i]);
                        imageSave[i].Save(imageSaveEnd, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                   
                }
                catch 
                {

                }

            }
        }
    }
}
