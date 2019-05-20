using System;
using System.Collections.Generic;
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

namespace Login
{
    [Serializable]
    public class Person
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        public string Number { get; set; }
        public string PathImg { get; set; }

        public string Company { get; set; }
        public string Adress { get; set; }

        public string Email { get; set; }

        public string Passw { get; set; }
        public Person()
        {

        }
        public Person(string firstN,string SecondN,string number,string Path, string company,string adress,string email,string passwd)
        {
            FirstName = firstN;
            SecondName = SecondN;
            Number = number;
            PathImg = Path;
            Company = company;
            Adress = adress;
            Email = email;
             Passw = passwd;
        }
    }
}
