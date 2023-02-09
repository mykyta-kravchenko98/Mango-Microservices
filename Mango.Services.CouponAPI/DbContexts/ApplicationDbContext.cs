using Mango.Services.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.DbContexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Coupon>().HasData(new Coupon()
        {
            CouponId = 1,
            CouponCode = "5OFF",
            DiscountAmount = 5
        });
        
        modelBuilder.Entity<Coupon>().HasData(new Coupon()
        {
            CouponId = 2,
            CouponCode = "10OFF",
            DiscountAmount = 10
        });
        
        modelBuilder.Entity<Coupon>().HasData(new Coupon()
        {
            CouponId = 3,
            CouponCode = "15OFF",
            DiscountAmount = 15
        });
        
        modelBuilder.Entity<Coupon>().HasData(new Coupon()
        {
            CouponId = 4,
            CouponCode = "20OFF",
            DiscountAmount = 20
        });
    }
}