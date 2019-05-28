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
using System.Globalization;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Bogus;

namespace MVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int countPage=0;

        ObservableCollection<User> users = new ObservableCollection<User>();
        int currentPage = 1;
        int countItemPage = 100;
        SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};version=3;new=False;datetimeformat=CurrentCulture");
        List<int> Pages = new List<int>();
        public MainWindow()
        {
            InitializeComponent();
            SearchUsers();
            GenerateButtonSimple(countPage);
                //dgViewDB.ItemsSource = users;
            //  FillDataGrid();

        }

        private void SearchUsers(string search = "")
        {

            int beginItem = countItemPage * (currentPage - 1);
            int countUsersDB = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            users.Clear();

            con.Open();
            string query = "SELECT COUNT(*) as countUsers FROM tblUsers";
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                countUsersDB = int.Parse(reader["countUsers"].ToString());
            }
            //  string q = "SELECT Id, Name, Age,DayOfBir From tblUsers  ";

            reader.Close();
            //Запит до БД на вибірку інформації
            query = $"SELECT Id, Name, DayOfBir From tblUsers "+
               $"ORDER BY Id LIMIT {countItemPage} OFFSET {beginItem}";
            //Створити клас для виконанян команди по підключенню до БД
            cmd.CommandText = query;// = new SQLiteCommand(query, con);
            //Виконує команду і отримує об'єкт Reader для читання інформації з БД
            reader = cmd.ExecuteReader();

            //-----------------
            while (reader.Read())
            {
                int id = int.Parse(reader["Id"].ToString());
                User user = new User
                {
                    Id = id,
                    Name = reader["Name"].ToString(),
                    Date = DateTime.Parse(reader["DayOfBir"].ToString(), new CultureInfo("ru-RU")),
                    PathImg=img.Name
                };
                users.Add(user);

            }
            con.Close();
            con.Open();

            
            System.Data.DataSet dataSet = new System.Data.DataSet();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(query, con);
            dataAdapter.Fill(dataSet);
            dgViewDB.ItemsSource = dataSet.Tables[0].DefaultView;
            con.Close();
          

             countPage = countUsersDB / countItemPage;


        }
        private void Generation()
        {
            con.Open();
            var userFaker = new Faker<User>("uk")
                .RuleFor(o => o.Name, f => f.Name.FirstName())
                .RuleFor(o=>o.Date,f=>f.Date.Between(new DateTime(1950, 1, 1),  DateTime.Now))
                .RuleFor(o=>o.PathImg,f=>f.Image.Locale);

            var list = userFaker.Generate(5000);
            foreach (var user in list)
            {
                string nameGroup = user.Name;
          
                string query = $"Insert into tblUsers(Name,DayOfBir,Img) values('{nameGroup}','{user.Date}','{user.PathImg}')";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            con.Close();
            SearchUsers();
        }
        private void GenerateButton(int count,bool c )
        {
            wpPaginationButtons.Children.Clear();
            if(c==true)
            for (int i = currentPage-5; i <= 4+ currentPage; i++)
            {
               
                Button btn = new Button();
                btn.Height = 25;
                btn.Width = 40;
                btn.Tag = i;
                btn.Content = i;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Margin = new Thickness(5, 5, 5, 5);
                Pages.Add(i);
                wpPaginationButtons.Children.Add(btn);
                btn.Click += Btn_Click;
                if (i == count)
                    break;
            }
           else  if(c==false)
            {
                int j = currentPage-5;
                for (int i = 5 + currentPage; i >= currentPage-5; i--)
                {
                    if (i < 1 ||j < 1)
                    {
                        j++;
                        continue;
                    }
                        Button btn = new Button();
                    btn.Height = 25;
                    btn.Width = 40;
                    btn.Tag = j;
                    btn.Content = j;
                    btn.VerticalAlignment = VerticalAlignment.Top;
                    btn.Margin = new Thickness(5, 5, 5, 5);
                    Pages.Add(j);
                    wpPaginationButtons.Children.Add(btn);
                    btn.Click += Btn_Click;
                    j++;
                }
            }
        }
        private void GenerateButtonSimple(int count)
        {
            wpPaginationButtons.Children.Clear();
           
                for (int i = currentPage; i <= 9 + currentPage; i++)
                {

                    Button btn = new Button();
                    btn.Height = 25;
                    btn.Width = 40;
                    btn.Tag = i;
                    btn.Content = i;
                    btn.VerticalAlignment = VerticalAlignment.Top;
                    btn.Margin = new Thickness(5, 5, 5, 5);
                    Pages.Add(i);
                    wpPaginationButtons.Children.Add(btn);
                    btn.Click += Btn_Click;
                    if (i == count)
                        break;
                }
               
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            currentPage = int.Parse(btn.Tag.ToString());
            SearchUsers();
            if (currentPage == Pages[Pages.Count - 1] && countPage != currentPage)
            {
                Pages.Clear();
                GenerateButton(countPage, true);

            }
          else   if (currentPage == Pages[0]&& Pages[0]!=1)
            {
                Pages.Clear();
                GenerateButton(countPage, false);
            }

        }

        private void FillDataGrid()
        {
            con.Open();

            string q = "SELECT Id, Name,DayOfBir From tblUsers  ";
          System.Data.DataSet dataSet = new System.Data.DataSet();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(q, con);
            dataAdapter.Fill(dataSet);
            dgViewDB.ItemsSource = dataSet.Tables[0].DefaultView;
            con.Close();

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

            string query = $"Insert into tblUsers(Name,DayOfBir,Img) values('{name}','{ BDate.SelectedDate}','{ img.Name}')";
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            cmd.ExecuteNonQuery();
            con.Close();
            MessageBox.Show(BDate.SelectedDate.ToString());
            FillDataGrid();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Generation();
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
