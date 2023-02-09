using System.ComponentModel.DataAnnotations.Schema;
using Mango.Web.Models.Dtos;

namespace Mango.Web.Models.Dto;

public class CartDetailDto
{
    public long CartDetailId { get; set; }
    public long CartHeaderId { get; set; }
    public virtual CartHeaderDto CartHeaderDto { get; set; }
    public long ProductId { get; set; }
    public virtual ProductDto Product { get; set; }
    public int Count { get; set; }
}