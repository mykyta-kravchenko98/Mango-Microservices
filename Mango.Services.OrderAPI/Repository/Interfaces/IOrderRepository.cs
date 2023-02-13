using Mango.Services.OrderAPI.Models;

namespace Mango.Services.OrderAPI.Repository.Interfaces;

public interface IOrderRepository
{
    Task<bool> AddOrder(OrderHeader orderHeader);
    Task UpdateOrderStatusOfPayment(long orderHeaderId, PaymentStatus paymentStatus);
}