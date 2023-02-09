using Mango.Services.CouponAPI.Models.Dtos;
using Mango.Services.CouponAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers;

[Authorize]
[Route("api/coupon")]
public class CouponController
{
    protected ResponseDto _response;
    private readonly ICouponRepository _couponRepository;
    
    public CouponController(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
        _response = new ResponseDto();
    }
    
    [HttpGet]
    [Route("DiscountForCode/{code}")]
    public async Task<ResponseDto> GetDiscountForCode(string code)
    {
        try
        {
            var couponDto = await _couponRepository.GetCouponByCodeAsync(code);
            _response.Result = couponDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>()
            {
                ex.ToString()
            };
        }

        return _response;
    }
}