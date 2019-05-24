using Image.Help;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Login
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<Person> people = new List<Person>();
        string imageFolderSave = "Image";
        string PathImagDic;
        string CurImgUser;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            PathImagDic = System.IO.Path.Combine(Directory.GetCurrentDirectory(), imageFolderSave);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlSerializer dcjs = new XmlSerializer(typeof(List<Person>));

                using (StreamReader fs = new StreamReader("people.xml"))
                {
                    people = (List<Person>)dcjs.Deserialize(fs);

                }

            }
            catch 
            {
            

            }

        }

        LogWindow window;

        OpenFileDialog dlg;
        private  void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlSerializer dcjs = new XmlSerializer(typeof(List<Person>));
            FileStream fs = new FileStream("people.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            dcjs.Serialize(fs, people);
            fs.Close();

        }
        string ImgName;
        public bool c;
        public void LogIn()
        {
             c = false;
            foreach (var item in people)
            {
            if (window.Email==item.Email&&window.Passwd==item.Passw)
                {
                    c = true;
                    txtFirstName.Text = item.FirstName;
                    txtSecondName.Text = item.SecondName;

                    txtNumber.Text = item.Number;
                 //   dlg.FileName = item.PathImg;
                    txtCompany.Text =  item.Company;
                    txtAdres.Text = item.Adress;
                    txtEmail.Text = item.Email;
                    txtPassw.Password = item.Passw;
                    ImgName = item.PathImg;
                    img.Source = new BitmapImage(new Uri(Path.Combine(PathImagDic,  item.PathImg)));
                    CurImgUser = (Path.Combine(PathImagDic, item.PathImg));
                    MessageBox.Show("OK");
                    break;
                }
            }
         if(c==false)
            {
                MessageBox.Show("Error");
             
            }

        }


        private void BtnAddImg_Click(object sender, RoutedEventArgs e)
        {
            dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) " +
                "| *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dlg.ShowDialog() == true)
            {
                img.Source = new BitmapImage(new Uri(dlg.FileName));
                try
                {
                    
                    var filePath = dlg.FileName;
                    var image = System.Drawing.Image.FromFile(dlg.FileName);
                    ImgName = Guid.NewGuid().ToString() + ".jpg";
                    File.Copy(filePath,  System.IO.Path.Combine(Directory.GetCurrentDirectory() ,imageFolderSave,ImgName ));
                    if (!Directory.Exists(imageFolderSave))
                    {
                        Directory.CreateDirectory(imageFolderSave);
                    }
                    var bmpOrigin = new System.Drawing.Bitmap(image);
               
                    var imageSave = ImageWorker.CreateImage(bmpOrigin, int.Parse((cmbSize.SelectedItem as ComboBoxItem).Content.ToString()), int.Parse((cmbSize.SelectedItem as ComboBoxItem).Content.ToString()));
                    if (imageSave == null)
                        throw new Exception("Проблема обробки фото");

                    var imageSaveEnd = System.IO.Path.Combine(imageFolderSave, (cmbSize.SelectedItem as ComboBoxItem).Content.ToString()+"_"+ ImgName);
                    imageSave.Save(imageSaveEnd, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
               catch (Exception ex)
                {
                    MessageBox.Show($"Щось пішло не так {ex.Message}");
                }
            }
        }
        private void BtnLogIn_Click(object sender, RoutedEventArgs e)
        {
            window = null;
            window = new LogWindow(this);
            window.ShowDialog();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool c = false;
            if (txtFirstName.Text != "" && txtSecondName.Text != "" && txtNumber.Text != "" && txtCompany.Text != "" && txtAdres.Text != "" && txtEmail.Text != "" && txtPassw.Password != "" && img.Source != null)
            {
                foreach (var item in people)
                {
                    if (txtEmail.Text == item.Email)
                    {
                        c = true;
                        MessageBox.Show("Даний емeйл уже використовується");
                        break;
                    }
                }
                if (c == false)
                {
                    people.Add(new Person(txtFirstName.Text, txtSecondName.Text, txtNumber.Text, ImgName, txtCompany.Text, txtAdres.Text, txtEmail.Text, txtPassw.Password));
                    txtAdres.Clear();
                    txtCompany.Clear();
                    txtEmail.Clear();
                    txtFirstName.Clear();
                    txtNumber.Clear();
                    txtPassw.Clear();
                    txtSecondName.Clear();
                    img.Source = null;
                    MessageBox.Show("Користувача додано");
                }
            }
            else
            {
                MessageBox.Show("Потрібно заповнити всі поля");

            }
        }

        private void CmbSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           try
            {


                var image = System.Drawing.Image.FromFile(CurImgUser);

                var bmpOrigin = new System.Drawing.Bitmap(image);
                var imageSave = ImageWorker.CreateImage(bmpOrigin, int.Parse((cmbSize.SelectedItem as ComboBoxItem).Content.ToString()), int.Parse((cmbSize.SelectedItem as ComboBoxItem).Content.ToString()));
                if (imageSave == null)
                    throw new Exception("Проблема обробки фото");

                var imageSaveEnd = System.IO.Path.Combine(imageFolderSave, (cmbSize.SelectedItem as ComboBoxItem).Content.ToString() + "_" + ImgName);
                imageSave.Save(imageSaveEnd, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch 
            {
               
            }
        }
    }
}
