using Microsoft.EntityFrameworkCore;
namespace Backend.Models
{
    public class MyContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Depts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Filename=./db.sqlite3");
        }

        public MyContext(DbContextOptions<MyContext> options)
      : base(options)
        { }
    }
}