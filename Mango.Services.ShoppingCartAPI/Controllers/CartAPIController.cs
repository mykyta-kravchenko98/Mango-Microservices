using Mango.Kafka.Services.Interfaces;
using Mango.MessageBus;
using Mango.RabbitMQ;
using Mango.RabbitMQ.Enums;
using Mango.RabbitMQ.Services.Interfaces;
using Mango.Services.ShoppingCartAPI.Messages;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Topics = Mango.Kafka.Topics;

namespace Mango.Services.ShoppingCartAPI.Controllers;

[Authorize]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IMessageBus _messageBus;
    private readonly IMessageProducer _rabbitProducer;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ResponseDto _response;

    public CartController(ICartRepository cartRepository, IMessageBus messageBus,
        ICouponRepository couponRepository, IMessageProducer rabbitProducer, IKafkaProducer kafkaProducer)
    {
        _cartRepository = cartRepository;
        _messageBus = messageBus;
        _couponRepository = couponRepository;
        _rabbitProducer = rabbitProducer;
        _kafkaProducer = kafkaProducer;
        _response = new ResponseDto();
    }

    [HttpGet("GetCart/{userId}")]
    public async Task<ResponseDto> GetCart(string userId)
    {
        try
        {
            var cartDto = await _cartRepository.GetCartByUserId(userId);
            _response.Result = cartDto;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpPost("AddCart")]
    public async Task<ResponseDto> AddCart([FromBody]CartDto cartDto)
    {
        try
        {
            var result = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpPost("UpdateCart")]
    public async Task<ResponseDto> UpdateCart([FromBody]CartDto cartDto)
    {
        try
        {
            var result = await _cartRepository.CreateUpdateCart(cartDto);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpPost("ApplyCoupon")]
    public async Task<ResponseDto> ApplyCoupon([FromBody]CartDto cartDto)
    {
        try
        {
            var result = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpDelete("DeleteCart/{cartDetailId}")]
    public async Task<ResponseDto> DeleteCart(long cartDetailId)
    {
        try
        {
            var result = await _cartRepository.RemoveFromCart(cartDetailId);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpDelete("ClearCart/{userId}")]
    public async Task<ResponseDto> ClearCart(string userId)
    {
        try
        {
            var result = await _cartRepository.ClearCart(userId);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpDelete("RemoveCoupon/{userId}")]
    public async Task<ResponseDto> RemoveCoupon(string userId)
    {
        try
        {
            var result = await _cartRepository.RemoveCoupon(userId);
            _response.Result = result;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
    
    [HttpPost("Checkout")]
    public async Task<object> Checkout([FromBody]CheckoutHeaderDto checkoutHeader)
    {
        try
        {
            var cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
            if (cartDto is null)
            {
                return BadRequest();
            }

            if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
            {
                var couponVerificationDto = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
                if (checkoutHeader.DiscountTotal != couponVerificationDto.DiscountAmount)
                {
                    _response.IsSuccess = false;
                    _response.ErrorMessages = new List<string>()
                        { "Coupon discount has changed, please confirm action." };
                    _response.DisplayMessage = "Coupon discount has changed, please confirm action.";
                    return _response;
                }
            }
            
            checkoutHeader.CartDetails = cartDto.CartDetails;

            //Uncommit if you want Azure Service Bus
            //await _messageBus.PublishMessage(checkoutHeader, Topics.CheckoutMessage);
            
            //RabbitMQ realization
            //_rabbitProducer.PublishMessage(checkoutHeader, Queue.Checkout);
            
            //Kafka realization
            await _kafkaProducer.PublishMessage(checkoutHeader, Topics.CheckoutMessage);

            await _cartRepository.ClearCart(checkoutHeader.UserId);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }
}