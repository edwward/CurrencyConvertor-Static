using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvertor_Static
{
    public class Root   //Root class is the main class. API return rates in the rates. It returns all currency name with value.
    {
        public Rate rates { get; set; }
    }
}
