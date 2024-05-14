using Microsoft.EntityFrameworkCore;
using OAM = Order.API.Models;
namespace Order.API.Context
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions options) : base(options)
        {

        }


        public DbSet<OAM.Order> Orders { get; set; }
        public DbSet<OAM.OrderItem> OrderItems { get; set; }

    }
}
