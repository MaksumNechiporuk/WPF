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
        DateTime date;
        public DateTime Date
        {
            get { return this.date; }

            set
            {
                if (this.date != value)
                {
                    this.date = value;
                    this.NotifyPropertyChanged("date");
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
            Date = d;
            PathImg = path;
        }
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

      
    }
}
