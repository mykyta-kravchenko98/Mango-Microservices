using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.Dto;

public class CartHeaderDto
{
    public long CartHeaderId { get; set; }
    public string UserId { get; set; }
    public string CouponCode { get; set; }
    public double OrderTotal { get; set; }
    public double DiscountTotal { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DataType PickupDataType { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string CardNumber { get; set; }
    public string CVV { get; set; }
    public string ExpireMonthYear { get; set; }
}