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
        string ImgName;
        string imageFolderSave = "Image";
        string PathImagDic;
        DataRowView d;


        ObservableCollection<User> users = new ObservableCollection<User>();
        int currentPage = 1;
        DateTime searchDate;
        int countItemPage = 100;
        SQLiteConnection con = new SQLiteConnection($"Data source={"dbUsers.sqlite"};datetimeformat=CurrentCulture");
        List<int> Pages = new List<int>();
        bool c = false;
        public MainWindow()
        {
            InitializeComponent();
           // Generation();
            PathImagDic = System.IO.Path.Combine(Directory.GetCurrentDirectory(), imageFolderSave);

            SearchUsers();
            GenerateButtonSimple(countPage);
            //    MessageBox.Show(PathImagDic);
        }
        private void SearchUsers()
        {
            string searchName = txtName.Text;

            bool c1 = false;
            int beginItem = countItemPage * (currentPage - 1);
            int countUsersDB = 0;
            users.Clear();
            con.Open();


            string query = "SELECT COUNT(*) as countUsers FROM tblUsers";
           
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            

            if (!string.IsNullOrEmpty(searchName))
            {
                query += $" WHERE Name LIKE '%{searchName}%'";
                if (c == true)
                {
                    query += $"  AND DayOfBir LIKE '%{searchDate}%'";
         
                    c = false;
                    c1 = true;
                }
            }
            if (c== true)
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
           
          // try
            {

                query = $"  SELECT Id, Name, DayOfBir, Image From tblUsers ";
                query += $" WHERE Image  NOT LIKE '%http%' AND   {beginItem}  <= Id  AND  {beginItem+ countItemPage}  >= Id ";
                cmd.CommandText = query;

                reader = cmd.ExecuteReader();
                ImageFromDirectory(reader);
                reader.Close();

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

            }

          // catch { }
            con.Close();
            dgViewDB.ItemsSource = users;
            countPage = countUsersDB / countItemPage;
            countPage++;
            c = false;
        }
        void ImageFromDirectory(SQLiteDataReader reader)
        {
               // MessageBox.Show(System.IO.Path.Combine(PathImagDic, ImgName));
            string []imageNames=Directory.GetFiles(PathImagDic);
            int i = 0;
            while (reader.Read())
            {
                int id = int.Parse(reader["Id"].ToString());
                User user = new User
                {
                    Id = id,
                    Name = reader["Name"].ToString(),
                    Birthday = DateTime.Parse(reader["DayOfBir"].ToString(), new CultureInfo("ru-RU")),
                    PathImg = System.IO.Path.Combine(PathImagDic,reader["Image"].ToString())
            };
                user.Birthday = DateTime.Parse(user.Birthday.ToShortDateString(), new CultureInfo("ru-RU"));
                users.Add(user);
            }
            //foreach (var item1 in imageNames)
            //foreach (var item in users)
            //{
            //        if(item.PathImg==item1)
            //    item.PathImg = System.IO.Path.Combine(PathImagDic, item1);

            //i++;
            //}
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
           // MessageBox.Show(users[0].PathImg);
        }
        private void Generation()
        {
            con.Open();
            var userFaker = new Faker<User>("uk")
                .RuleFor(o => o.Name, f => f.Name.FirstName())
                .RuleFor(o=>o.Birthday, f=>f.Date.Between(new DateTime(1950, 1, 1),  DateTime.Now))
                .RuleFor(o=>o.PathImg,f=> f.Internet.Avatar());
            //PicsumUrl()
            var list = userFaker.Generate(199);
            foreach (var user in list)
            {
                string name = user.Name;
                DateTime date= DateTime.Parse(user.Birthday.ToShortDateString(), new CultureInfo("ru-RU"));

                string query = $"Insert into tblUsers(Name,DayOfBir,Image) values('{name}','{date}','{user.PathImg}')";
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
                       ImgName = Guid.NewGuid().ToString() + ".jpg";
                    File.Copy(filePath, System.IO.Path.Combine(imageFolderSave, ImgName));
                    if (!Directory.Exists(imageFolderSave))
                    {
                        Directory.CreateDirectory(imageFolderSave);
                    }
                    var bmpOrigin = new System.Drawing.Bitmap(image);
                   // ImgName= dlg.SafeFileName ;
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

            string query = $"Insert into tblUsers(Name,DayOfBir,Image) values('{name}','{ BDate.SelectedDate}','{ ImgName}')";
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            cmd.ExecuteNonQuery();
            con.Close();
      
            c = false;
            txtName.Clear();
            BDate.Text = null;


            img.Source = null;
            SearchUsers();
        }

        private void BDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                searchDate = BDate.SelectedDate.Value;
                c = true;
            }
            catch { }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchUsers();
            txtName.Clear();
            BDate.Text = null;


            img.Source = null;
            GenerateButtonSimple(countPage);
            btnShow.IsEnabled = true;
         //   BDate.ClearValue();


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

        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
              try
            {
                if (dgViewDB.SelectedItem != null)
                {
                    int ind = 0;
                    ind = users.IndexOf(dgViewDB.SelectedItem as User);
                    

                    SQLiteCommand cmd;
                    con.Open();
                    string query = $"Delete FROM tblUsers where Name='{users[ind].Name}'";
                    cmd = new SQLiteCommand(query, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    users.RemoveAt(ind);
                    SearchUsers();
                }
            }
            catch
            {
            }
        }
        private void DgViewDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                
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
                    // ImgName= dlg.SafeFileName ;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Щось пішло не так {ex.Message}");
                }
            }
            MessageBox.Show(dgViewDB.Items[0].ToString());

            if (dgViewDB.SelectedItem != null)
            {
                int ind = 0;
                ind = users.IndexOf(dgViewDB.SelectedItem as User);


                con.Open();
                SQLiteCommand cmd = new SQLiteCommand("UPDATE tblUsers Set Image=@Image Where Id=@Id", con);
                cmd.Parameters.AddWithValue("Id", users[ind].Id);
                cmd.Parameters.AddWithValue("Image", ImgName);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Record Update Successfully", "Updated", MessageBoxButton.OK);
                SearchUsers();
            }
        }
    }

   
}
