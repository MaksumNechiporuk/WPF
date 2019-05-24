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
using System.Windows.Shapes;

namespace Login
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        MainWindow window;
        public Window1(MainWindow main)
        {
            InitializeComponent();
            window = main;
        }
        public string Email { get; set; }
        public string Passwd { get; set; }

        private void BtnLog_Click(object sender, RoutedEventArgs e)
        {
            Email = txtEmail.Text;
            Passwd = txtPasswd.Password;
            window.LogIn();
            if (window.c == true)
                Close();
        }

       
    }
}
