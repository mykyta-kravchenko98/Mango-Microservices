using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContext;

    public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> AddOrder(OrderHeader orderHeader)
    {
        await using var _db = new ApplicationDbContext(_dbContext);
        _db.OrderHeaders.Add(orderHeader);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task UpdateOrderStatusOfPayment(long orderHeaderId, PaymentStatus paymentStatus)
    {
        await using var _db = new ApplicationDbContext(_dbContext);
        var orderHeaderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(oh => oh.OrderHeaderId == orderHeaderId);
        if (orderHeaderFromDb is not null)
        {
            orderHeaderFromDb.StatusOfPayment = paymentStatus;
            await _db.SaveChangesAsync();
        }
    }
}