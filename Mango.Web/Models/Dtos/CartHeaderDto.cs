using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.Dto;

public class CartHeaderDto
{
    public long CartHeaderId { get; set; }
    public string UserId { get; set; }
    public string CouponCode { get; set; }
    public double OrderTotal { get; set; }
}