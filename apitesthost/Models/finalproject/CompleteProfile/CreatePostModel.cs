using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apitesthost.Models.finalproject.CompleteProfile
{
    public class CreatePostModel
    {
        public int ID { get; set; }
        public int postID { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string create_date { get; set; }
        public string experience_level { get; set; }
        public string email_address { get; set; }
        public List<string> skills { get; set; }
        public string photo_url { get; set; }
    }
}
