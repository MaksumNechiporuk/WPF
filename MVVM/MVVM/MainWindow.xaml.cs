using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
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
      
        SQLiteConnection con = new SQLiteConnection($"Data Source={"dbUsers.sqlite"}");
        public MainWindow()
        {
            InitializeComponent();
            FillDataGrid();

        }

        private void FillDataGrid()
        {
            con.Open();

            string q = "SELECT * From tblUsers";
            DataSet dataSet = new DataSet();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(q, con);
            dataAdapter.Fill(dataSet);
            dgViewDB.ItemsSource = dataSet.Tables[0].DefaultView;
            con.Close();

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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

            con.Open();
            string name = txtName.Text;
            string age = sldAge.Value.ToString();

            string query = $"Insert into tblUsers(Name,Age,DayOfBir,Img) values('{name}','{ age}','{ BDate}','{ img.Uid}')";
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            cmd.ExecuteNonQuery();
            con.Close();
            FillDataGrid();
        }
    }

    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base($"Data Source={"dbUsers.sqlite"}")
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
