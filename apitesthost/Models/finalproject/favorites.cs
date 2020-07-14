using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apitesthost.Models.finalproject
{
    public class favorites
    {
        public int ID { get; set; }
        public int user_id { get; set; }
        public int favorited_user_id { get; set; }
    }
}
