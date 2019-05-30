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
        DateTime searchDate;
        int countItemPage = 100;
        SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};datetimeformat=CurrentCulture");
        List<int> Pages = new List<int>();
        public MainWindow()
        {
            InitializeComponent();
       //  Generation();
         SearchUsers();
            GenerateButtonSimple(countPage);
           //dgViewDB.ItemsSource = users;
        }
        private void SearchUsers()
        {
            string searchName = txtName.Text;
   
           //  searchDate=new DateTime(BDate.SelectedDate.Value.Year);
           //     searchDate = BDate.SelectedDate.Value;
            int beginItem = countItemPage * (currentPage - 1);
            int countUsersDB = 0;
           
            users.Clear();

            con.Open();
            string query = "SELECT COUNT(*) as countUsers FROM tblUsers";
            if (!string.IsNullOrEmpty(searchName))
            {
                query += $" WHERE Name LIKE '%{searchName}%'";
            }
            if (searchDate!=null)
            {
                query += $" WHERE Name LIKE '%{searchDate}%'";
            }
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                countUsersDB = int.Parse(reader["countUsers"].ToString());
            }

            reader.Close();
            query = $"SELECT Id, Name, DayOfBir, Image From tblUsers ";
            if (!string.IsNullOrEmpty(searchName))
            {
                query += $" WHERE Name LIKE '%{searchName}%'";
            }
            if (searchDate != null)
            {
                query += $" WHERE Name LIKE '%{searchDate}%'";
            }
            query += $"ORDER BY Id LIMIT {countItemPage} OFFSET {beginItem}";
            cmd.CommandText = query;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = int.Parse(reader["Id"].ToString());
                User user = new User
                {
                    Id = id,
                    Name = reader["Name"].ToString(),
                    Birthday = DateTime.Parse(reader["DayOfBir"].ToString(), new CultureInfo("ru-RU")),
                    PathImg =  reader["Image"].ToString()
                };
                users.Add(user);

            }
            con.Close();
            dgViewDB.ItemsSource = users;

          

            countPage = countUsersDB / countItemPage;
            countPage++;
        }
        private void Generation()
        {
            con.Open();
            var userFaker = new Faker<User>("uk")
                .RuleFor(o => o.Name, f => f.Name.FirstName())
                .RuleFor(o=>o.Birthday, f=>f.Date.Between(new DateTime(1950, 1, 1),  DateTime.Now))
                .RuleFor(o=>o.PathImg,f=> f.Internet.Avatar());
            //PicsumUrl()
            var list = userFaker.Generate(2000);
            MessageBox.Show(list[0].PathImg);
            foreach (var user in list)
            {
                string nameGroup = user.Name;
          
                string query = $"Insert into tblUsers(Name,DayOfBir,Image) values('{nameGroup}','{user.Birthday}','{user.PathImg}')";
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
            for (int k=0, i = currentPage-5; k<11;k++, i++)
            {
                    if (i < 1 )
                    {
                        i = 0;
                        continue;

                    }
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
                for (int k=0, i = 3 + currentPage; k<11;k++, i--)
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
                btn.Background = Brushes.White;
                btn.Click += Btn_Click;
                    if (i == count)
                        break;
                }
               
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            currentPage = int.Parse(btn.Tag.ToString());
            btn.Background = Brushes.Blue;
           
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
            SearchUsers();
            foreach (var item in wpPaginationButtons.Children)
            {
                if (int.Parse((item as Button).Tag.ToString()) == currentPage)
                {
                    (item as Button).Background = Brushes.Yellow;

                }
                else
                {
                    (item as Button).Background = Brushes.White;

                }
            }
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
            SearchUsers();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Generation();
        }

        private void DgViewDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

      
        private void BtnClick_Click(object sender, RoutedEventArgs e)
        {
            SearchUsers();
        }

        private void BDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            searchDate = BDate.SelectedDate.Value;
        }
    }

   
}
