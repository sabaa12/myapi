using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace apitesthost.Models.finalproject.CompleteProfile
{
    public class complete_profile
    {
        public int ID { get; set; }
        public string user_name { get; set; }
        public string email_address { get; set; }
        public string gender { get; set; }
        public int age { get; set; }
        public string photo_url { get; set; }
    }

}
