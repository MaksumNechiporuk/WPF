using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM
{
    public class User : INotifyPropertyChanged
    {
        int id;
        string name;
        public int Id
        {
            get { return this.id; }

            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.NotifyPropertyChanged("Id");
                }
            }
        }

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
        int age;
        public int Age
        {
            get { return this.age; }

            set
            {
                if (this.age != value)
                {
                    this.age = value;
                    this.NotifyPropertyChanged("Age");
                }
            }
        }
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
        public User()
        {

        }
        string img;
        public string PathImg
        {
            get { return this.img; }

            set
            {
                if (this.img != value)
                {
                    this.img = value;
                    this.NotifyPropertyChanged("PathImg");
                }
            }
        }
        public User(string n,int a,DateTime d,string path)
        {
            Name = n;
            Age = a;
            Birthday = d;
            PathImg = path;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

    }
}
