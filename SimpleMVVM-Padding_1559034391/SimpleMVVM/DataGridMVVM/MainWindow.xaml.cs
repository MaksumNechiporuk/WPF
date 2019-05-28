using Bogus;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
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

namespace DataGridMVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<User> users = new ObservableCollection<User>();
        int currentPage = 1;
        int countItemPage = 5;
        public MainWindow()
        {
            InitializeComponent();
            SearchUsers();
            dgSimple.ItemsSource = users;
        }
        private void SearchUsers(string search="")
        {
            int beginItem = countItemPage * (currentPage - 1);
            int countUsersDB = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            users.Clear();
            //Назва бази даних
            string dbName = "semen.sqlite";
            //Cтворюю обєкт для взаємодії із БД
            SQLiteConnection con = new SQLiteConnection($"Data Source={dbName}");
            //Відкриваю підключення
            con.Open();
            string query = "SELECT COUNT(*) as countGroups FROM tblGroups";
            SQLiteCommand cmd = new SQLiteCommand(query, con);
            SQLiteDataReader reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                countUsersDB=int.Parse(reader["countGroups"].ToString());
            }
            reader.Close();
            //Запит до БД на вибірку інформації
            query = $"Select Id, Name FROM tblGroups " +
                $"ORDER BY Id LIMIT {countItemPage} OFFSET {beginItem}";
            //Створити клас для виконанян команди по підключенню до БД
            cmd.CommandText = query;// = new SQLiteCommand(query, con);
            //Виконує команду і отримує об'єкт Reader для читання інформації з БД
            reader = cmd.ExecuteReader();

            //-----------------
            while(reader.Read())
            {
                int id= int.Parse(reader["Id"].ToString());
                User user = new User
                {
                    Id=id,
                    Name= reader["Name"].ToString(),
                    Birthday = new DateTime(1974, 1, 17),
                    ImageUrl = "https://i.vimeocdn.com/portrait/7224366_300x300"
                };
                users.Add(user);
            }
            con.Close();

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            statusBarLabelInfo.Content = elapsedTime+$" Count: {users.Count}";

            int countPage = countUsersDB / countItemPage;
            GenerateButton(10);

        }
        private void GenerateButton(int count)
        {
            wpPaginationButtons.Children.Clear();
            //PaginationButtons
            for (int i = 1; i <= count; i++)
            {
                Button btn = new Button();
                btn.Height = 25;
                btn.Width = 40;
                btn.Tag = i;
                btn.Content = i;
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.Margin = new Thickness(5, 5, 5, 5);

                wpPaginationButtons.Children.Add(btn);
                btn.Click += Btn_Click;
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            currentPage = int.Parse(btn.Tag.ToString());
            SearchUsers();
            //MessageBox.Show("You press button " + btn.Tag);
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            users.Add(new User()
            {
                Id = 3333,
                Name = "New user",
                Birthday = new DateTime(1991, 9, 2),
                ImageUrl = "https://sharerice.com/images/thumb/4/40/10931180_947171611960525_5370449655645132591_n.jpg/300px-10931180_947171611960525_5370449655645132591_n.jpg"
            });
        }

        private void BtnChangeUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgSimple.SelectedItem != null)
            {
                var userView = (dgSimple.SelectedItem as User);
                userView.Name = "Катюха";
                //var user = _context.User.SingleOrDefault(u => u.Id == userView.Id);
                //if(user!=null)
                //{
                //    user.Name = txtName.Text;
                //    _context.SaveChanges();
                //    userView.Name = user.Name;
                //}
            }
        }

        private void BtnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgSimple.SelectedItem != null)
                users.Remove(dgSimple.SelectedItem as User);
        }

        private void BtnGenerateUser_Click(object sender, RoutedEventArgs e)
        {

            string dbName = "semen.sqlite";
            SQLiteConnection con = new SQLiteConnection($"Data Source={dbName}");
            con.Open();
            var userFaker = new Faker<User>("uk")
               .RuleFor(o => o.Name, f => f.Company.CompanyName());
            var list=userFaker.Generate(10000);
            foreach (var user in list)
            {
                string nameGroup = user.Name;
                string query = $"Insert into tblGroups(Name) values('{nameGroup}')";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.ExecuteNonQuery();
            }
            con.Close();
            SearchUsers();
        }
    }
    public class User : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string name;
        private DateTime birthday;
        public DateTime Birthday
        {
            get { return this.birthday; }
            set
            {
                if (this.birthday != value)
                {
                    this.birthday = value;
                    this.NotifyPropertyChanged("Birthday");
                }
            }
        }
        public string ImageUrl { get; set; }

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
