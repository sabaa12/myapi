using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apitesthost.Models.finalproject.CompleteProfile
{
    public class developer_skills
    {
        public int ID { get; set; }

        public int PostID { get; set; }
        public int  employerID { get; set; }
        public string skill { get; set; }
    }
}
