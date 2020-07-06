using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apitesthost.Models
{
    public class users
    {
        public int ID { get; set; }
       
         public string email_address { get; set; }
        public string password { get; set; }
        public int role { get; set; }
        public Boolean iscomplete { get; set; }
    }
}
