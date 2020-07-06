using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apitesthost.Models.finalproject.CompleteProfile
{
    public class compteteprofilepost
    {
        public string user_name { get; set; }
        public string email_address { get; set; }
        public string age { get; set; }
        public string gender { get; set; }
        public string position { get; set; }
        public List<string> skills { get; set; }

        public string photo_url { get; set; }

        public string role { get; set; }
    }
}
