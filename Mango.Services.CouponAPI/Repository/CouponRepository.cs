using AutoMapper;
using Mango.Services.CouponAPI.DbContexts;
using Mango.Services.CouponAPI.Models.Dtos;
using Mango.Services.CouponAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Repository;

public class CouponRepository : ICouponRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CouponRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public async Task<CouponDto> GetCouponByCodeAsync(string couponCode)
    {
        if (string.IsNullOrEmpty(couponCode))
        {
            return new CouponDto();
        }

        var coupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);

        if (coupon is not null)
        {
            return _mapper.Map<CouponDto>(coupon);
        }

        return new CouponDto();
    }
}