using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Windows;
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

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
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
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);

            }

        }

        Window1 window;

        OpenFileDialog dlg = new OpenFileDialog();





        private void Button_Click(object sender, RoutedEventArgs e)
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
                if (c==false)
                {
                    people.Add(new Person(txtFirstName.Text, txtSecondName.Text, txtNumber.Text, dlg.FileName, txtCompany.Text, txtAdres.Text, txtEmail.Text, txtPassw.Password));
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
       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                dlg.Title = "Select a picture";
                if (dlg.ShowDialog() == true)
                {
                    img.Source = new BitmapImage(new Uri(dlg.FileName));

                }
            }
            catch
            {
           

            }
        }

        private  void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlSerializer dcjs = new XmlSerializer(typeof(List<Person>));
            FileStream fs = new FileStream("people.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            dcjs.Serialize(fs, people);
            fs.Close();

        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
         

            
        }
        public void LogIn()
        {
            bool c = false;
            foreach (var item in people)
            {
            if (window.Email==item.Email&&window.Passwd==item.Passw)
                {
                    c = true;
                    txtFirstName.Text = item.FirstName;
                    txtSecondName.Text = item.SecondName;

                    txtNumber.Text = item.Number;
                    dlg.FileName = item.PathImg;
                    txtCompany.Text =  item.Company;
                    txtAdres.Text = item.Adress;
                    txtEmail.Text = item.Email;
                    txtPassw.Password = item.Passw;
                    img.Source = new BitmapImage(new Uri(item.PathImg));
                    MessageBox.Show("OK");
                    break;
                }
            }
           // if(c==false)
             //   window.ShowDialog();

        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            window = null;
            window = new Window1(this);
            window.ShowDialog();
        }
    }
}
