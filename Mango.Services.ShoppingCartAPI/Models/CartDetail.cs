using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models;

public class CartDetail
{
    public long CartDetailId { get; set; }
    public long CartHeaderId { get; set; }
    [ForeignKey("CartHeaderId")]
    public virtual CartHeader CartHeader { get; set; }
    public long ProductId { get; set; }
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
    public int Count { get; set; }
}