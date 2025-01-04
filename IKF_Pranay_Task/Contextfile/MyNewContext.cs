using Microsoft.EntityFrameworkCore;
using IKF_Pranay_Task.Models;


namespace IKF_Pranay_Task.Contextfile
{
    public class MyNewContext:DbContext
    {
        public MyNewContext(DbContextOptions<MyNewContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
