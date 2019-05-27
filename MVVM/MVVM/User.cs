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
        public string Name
        {
            get { return this.Name; }

            set
            {
                if (this.Name != value)
                {
                    this.Name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        } 
       
        public int Age {
            get { return this.Age; }

            set
            {
                if (this.Age != value)
                {
                    this.Age = value;
                    this.NotifyPropertyChanged("Age");
                }
            }
        }

        public DateTime date
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
        public string PathImg
        {
            get { return this.PathImg; }

            set
            {
                if (this.PathImg != value)
                {
                    this.PathImg = value;
                    this.NotifyPropertyChanged("PathImg");
                }
            }
        }
        public User(string n,int a,DateTime d,string path)
        {
            Name = n;
            Age = a;
            date = d;
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
