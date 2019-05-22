using System;
using System.Collections.Generic;
using System.Data.SQLite;
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

namespace SqlLite
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



        [SQLiteFunction(Name = "Sha1", Arguments = 1, FuncType = FunctionType.Scalar)]
        public class Sha1 : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                var buffer = args[0] as byte[];

                if (buffer == null)
                {
                    var s = args[0] as string;

                    if (s != null)
                        buffer = Encoding.Unicode.GetBytes(s);
                }

                if (buffer == null)
                    return null;

                using (var sha1 = System.Security.Cryptography.SHA1.Create())
                {
                    return sha1.ComputeHash(buffer);
                }
            }
        }

        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            string dbName = "Users.sqlite";
            SQLiteConnection con = new SQLiteConnection($"Data Source={dbName}");
            con.Open();
            string Login = txtLogin.Text;
            string pass = txtPassword.Text;

            string query = $"Insert into Artists(Login) values('{Login}')";

            SQLiteCommand cmd = new SQLiteCommand(query, con);
             query = $"Insert into Artists(Password) values('{ pass}')";
         
            cmd.ExecuteNonQuery();
            con.Close();
           // UpdateGrid();
        }
    }
}
