using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CouponAPI.Models;

public class Coupon
{
    [Key]
    public long CouponId { get; set; }
    public string CouponCode { get; set; }
    public double DiscountAmount { get; set; }
}