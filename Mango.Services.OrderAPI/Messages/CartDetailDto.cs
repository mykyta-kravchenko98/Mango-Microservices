using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderAPI.Messages;

public class CartDetailDto
{
    public long CartDetailId { get; set; }
    public long CartHeaderId { get; set; }
    public long ProductId { get; set; }
    public virtual ProductDto Product { get; set; }
    public int Count { get; set; }
}