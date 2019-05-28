using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace SimpleMVVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        public MainWindow()
        {
            InitializeComponent();
            lbUsers.ItemsSource = _users;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _users.Add(new User { Id = 2, Name = "Валера" });
        }
    }

    public class User : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        public string Name
        {
            get { return this._name; }
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }
        public int Id
        {
            get { return this._id; }
            set
            {
                if (this._id != value)
                {
                    this._id = value;
                    this.NotifyPropertyChanged("Id");
                }
            }
        }
        public string FullInfo { get { return String.Format("{0} - {1}", Id, Name); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if(this.PropertyChanged!=null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
