using apitesthost.Models.finalproject.CompleteProfile;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apitesthost.Models
{
    public class MvcMovieContext : DbContext
    {
        public MvcMovieContext(DbContextOptions<MvcMovieContext> options)
            : base(options)
        {
        }

        
        public DbSet<users> users { get; set; }
        public DbSet<complete_profile> complete_profile { get; set; }
        public DbSet<developer> developer { get; set; }
        public DbSet<skkils> skkils { get; set; }
    }
}
