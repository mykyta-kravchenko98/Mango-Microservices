using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models.Dto;

public class CartDetailDto
{
    public long CartDetailId { get; set; }
    public long CartHeaderId { get; set; }
    public virtual CartHeaderDto CartHeaderDto { get; set; }
    public long ProductId { get; set; }
    public virtual Product Product { get; set; }
    public int Count { get; set; }
}