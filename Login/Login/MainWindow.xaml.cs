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

namespace Login
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog myDialog = new OpenFileDialog();
        List<Person> people = new List<Person>();
        public MainWindow()
        {
            InitializeComponent();
        }

        

        private void Img_MouseEnter(object sender, MouseEventArgs e)
        {

            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "Select a picture";
                dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png";

                if   (dlg.ShowDialog()==true)
                {
                    img.Source = new BitmapImage(new Uri(dlg.FileName));

                }

            }
            catch
            {
                MessageBox.Show("asdas");

            }
        }

        private void BrdSiteLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            people.Add(new Person(txtFirstName.Text, txtSecondName.Text, txtNumber.Text, img, txtCompany.Text, txtAdres.Text, txtEmail.Text, txtPassw.ToString()));
        }
    }
}
