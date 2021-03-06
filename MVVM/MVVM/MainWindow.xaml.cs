﻿using Microsoft.Win32;
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
        int countPage = 0;
      public  string ImgName;
        string imageFolderSave = "Image";
        string PathImagDic;

        UserService service;
   public     ObservableCollection<User> users = new ObservableCollection<User>();
        int currentPage = 1;
        DateTime searchDate;
        int countItemPage = 100;
        List<int> Pages = new List<int>();
       public bool check = false;
        public MainWindow()
        {
            InitializeComponent();
            // Generation();
            service = new UserService(this);
            PathImagDic = System.IO.Path.Combine(Directory.GetCurrentDirectory(), imageFolderSave);

            SearchUsers();
            GenerateButtonSimple(countPage);
        }
        public void SearchUsers()
        {
            string searchName = txtName.Text;

            bool c1 = false;
            int beginItem = countItemPage * (currentPage - 1);
            int countUsersDB = 0;
            users.Clear();
            SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};datetimeformat=CurrentCulture");
            con.Open();


            string query = "SELECT COUNT(*) as countUsers FROM tblUsers";

            SQLiteCommand cmd = new SQLiteCommand(query, con);
            SQLiteDataReader reader = cmd.ExecuteReader();


            if (!string.IsNullOrEmpty(searchName))
            {
                query += $" WHERE Name LIKE '%{searchName}%'";
                if (check == true)
                {
                    query += $"  AND DayOfBir LIKE '%{searchDate}%'";

                    check = false;
                    c1 = true;
                }
            }
            if (check == true)
            {
                query += $"  WHERE DayOfBir LIKE '%{searchDate}%'";
                c1 = true;
            }
            cmd = new SQLiteCommand(query, con);
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                countUsersDB = int.Parse(reader["countUsers"].ToString());
            }
            reader.Close();

            try
            {


                query = $"SELECT Id, Name, DayOfBir, Image From tblUsers ";
                query += $" WHERE Image LIKE '%http%' ";

                if (!string.IsNullOrEmpty(searchName))
                {
                    query += $" AND  Name LIKE '%{searchName}%' ";
                }
                if (c1 == true)
                {
                    query += $" AND  DayOfBir LIKE '%{searchDate}%' ";
                }
                query += $"  ORDER BY Id LIMIT {countItemPage} OFFSET {beginItem}";
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                ImageFromEthernet(reader);
                reader.Close();
                query = $"  SELECT Id, Name, DayOfBir, Image From tblUsers ";
                query += $" WHERE Image  NOT LIKE '%http%' AND   {beginItem}  <= Id  AND  {beginItem + countItemPage}  >= Id ";
                if (!string.IsNullOrEmpty(searchName))
                {
                    query += $" AND  Name LIKE '%{searchName}%' ";
                }
                if (c1 == true)
                {
                    query += $" AND  DayOfBir LIKE '%{searchDate}%' ";
                }
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                ImageFromDirectory(reader);
                reader.Close();
            }

            catch { }
            con.Close();
            dgViewDB.ItemsSource = users;
            countPage = countUsersDB / countItemPage;
            countPage++;
            check = false;
        }
        void ImageFromDirectory(SQLiteDataReader reader)
        {
            while (reader.Read())
            {
                int id = int.Parse(reader["Id"].ToString());
                User user = new User
                {
                    Id = id,
                    Name = reader["Name"].ToString(),
                    Birthday = DateTime.Parse(reader["DayOfBir"].ToString(), new CultureInfo("ru-RU")),
                    PathImg = System.IO.Path.Combine(PathImagDic, reader["Image"].ToString())
                };
                user.Birthday = DateTime.Parse(user.Birthday.ToShortDateString(), new CultureInfo("ru-RU"));
                users.Add(user);
            }


        }
        void ImageFromEthernet(SQLiteDataReader reader)
        {
            while (reader.Read())
            {
                int id = int.Parse(reader["Id"].ToString());
                User user = new User
                {
                    Id = id,
                    Name = reader["Name"].ToString(),
                    Birthday = DateTime.Parse(reader["DayOfBir"].ToString(), new CultureInfo("ru-RU")),
                    PathImg = reader["Image"].ToString()
                };
                user.Birthday = DateTime.Parse(user.Birthday.ToShortDateString(), new CultureInfo("ru-RU"));
                users.Add(user);
            }
        }
        private void Generation()
        {
            SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};datetimeformat=CurrentCulture");
            con.Open();
            var userFaker = new Faker<User>("uk")
                .RuleFor(o => o.Name, f => f.Name.FirstName())
                .RuleFor(o => o.Birthday, f => f.Date.Between(new DateTime(1950, 1, 1), DateTime.Now))
                .RuleFor(o => o.PathImg, f => f.Internet.Avatar());
            var list = userFaker.Generate(3000);
            foreach (var user in list)
            {
                string name = user.Name;
                DateTime date = DateTime.Parse(user.Birthday.ToShortDateString(), new CultureInfo("ru-RU"));

                string query = $"Insert into tblUsers(Name,DayOfBir,Image) values('{name}','{date}','{user.PathImg}')";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            con.Close();
            SearchUsers();
        }
        private void GenerateButton(int count, bool c)
        {
            wpPaginationButtons.Children.Clear();
            if (c == true)
                for (int k = 0, i = currentPage - 5; k < 11; k++, i++)
                {
                    if (i < 1)
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
            else if (c == false)
            {
                int j = currentPage - 5;
                for (int k = 0, i = 3 + currentPage; k < 11; k++, i--)
                {
                    if (i < 1 || j < 1)
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
            else if (currentPage == Pages[0] && Pages[0] != 1)
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
                    ImgName = Guid.NewGuid().ToString() + ".jpg";
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
            service.Insert();
            
        }

        private void BDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                searchDate = BDate.SelectedDate.Value;
                check = true;
            }
            catch { }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchUsers();
            GenerateButtonSimple(countPage);
            txtName.Clear();
            BDate.Text = null;


            img.Source = null;
            btnShow.IsEnabled = true;


        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            BDate.Text = null;


            img.Source = null;
            SearchUsers();

            GenerateButtonSimple(countPage);
            btnShow.IsEnabled = false;

        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            service.Edit();

        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            service.Delete();
        }
       

        private void UserImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string imageFolderSave = "Image";
            Image image = sender as Image;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) " +
                "| *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

            if (dlg.ShowDialog() == true)
            {

                image.Source = new BitmapImage(new Uri(dlg.FileName));
                try
                {
                    var filePath = dlg.FileName;
                    var img = System.Drawing.Image.FromFile(dlg.FileName);
                    ImgName = Guid.NewGuid().ToString() + ".jpg";
                    File.Copy(filePath, System.IO.Path.Combine(imageFolderSave, ImgName));
                    if (!Directory.Exists(imageFolderSave))
                    {
                        Directory.CreateDirectory(imageFolderSave);
                    }
                    var bmpOrigin = new System.Drawing.Bitmap(img);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Щось пішло не так {ex.Message}");
                }
            }

            if (dgViewDB.SelectedItem != null)
            {
                int ind = 0;
                ind = users.IndexOf(dgViewDB.SelectedItem as User);

                SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};datetimeformat=CurrentCulture");
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand("UPDATE tblUsers Set Image=@Image Where Id=@Id", con);
                cmd.Parameters.AddWithValue("Id", users[ind].Id);
                cmd.Parameters.AddWithValue("Image", ImgName);
                cmd.ExecuteNonQuery();
                con.Close();
                SearchUsers();
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            service.Update();
        }
        public class UserService
        {
            MainWindow window;
            public UserService(MainWindow main)
            {
                window = main;
            }
            public void Insert()
            {
                SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};datetimeformat=CurrentCulture");
                con.Open();
                string name = window.txtName.Text;

                string query = $"Insert into tblUsers(Name,DayOfBir,Image) values('{name}','{ window.BDate.SelectedDate}','{  window.ImgName}')";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();

                window.check = false;
                window.txtName.Clear();
                window.BDate.Text = null;


                window.img.Source = null;
                window.SearchUsers();
            }
            public void Edit()
            {
                SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};datetimeformat=CurrentCulture");
                window.btnUpdate.IsEnabled = true;
                if (window.dgViewDB.SelectedItem != null)
                {
                    window.btnAdd.IsEnabled = false;
                    window.btnAddImg.IsEnabled = false;
                    window.btnSearch.IsEnabled = false;
                    int ind = 0;
                    ind = window.users.IndexOf(window.dgViewDB.SelectedItem as User);
                    window.lblId.Content = window.users[ind].Id;
                    window.txtName.Text = window.users[ind].Name;
                    window.img.Source = new BitmapImage(new Uri(window.users[ind].PathImg));
                }
            }
            public void Delete()
            {
                SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};datetimeformat=CurrentCulture");
                //  try
                {

                    if (window.dgViewDB.SelectedItem != null)
                    {
                        int ind = 0;
                        ind = window.users.IndexOf(window.dgViewDB.SelectedItem as User);


                        SQLiteCommand cmd;
                        con.Open();
                        string query = $"Delete FROM tblUsers where Id='{window.users[ind].Id}'";
                        cmd = new SQLiteCommand(query, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        window.users.RemoveAt(ind);
                        window.SearchUsers();
                    }
                }
                // catch
                {
                }
            }
            public void Update()
            {
                SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};datetimeformat=CurrentCulture");
                try
                {
                    con.Open();

                    SQLiteCommand cmd = new SQLiteCommand("UPDATE tblUsers Set Name=@Name  Where Id=@Id", con);
                    cmd.Parameters.AddWithValue("Id", window.lblId.Content);
                    cmd.Parameters.AddWithValue("Name", window.txtName.Text);
                    window.btnAdd.IsEnabled = true;
                    window.btnAddImg.IsEnabled = true;
                    window.btnSearch.IsEnabled = true;
                    window.btnAdd.IsEnabled = true;

                    cmd.ExecuteNonQuery();
                    con.Close();
                    window.txtName.Clear();
                    window.BDate.Text = null;


                    window.img.Source = null;
                    window.SearchUsers();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
    }
   

}
