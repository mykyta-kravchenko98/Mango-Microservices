namespace Mango.Services.ShoppingCartAPI.Models.Dtos;

public class CouponDto
{
    public long CouponId { get; set; }
    public string CouponCode { get; set; }
    public double DiscountAmount { get; set; }
}