using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repository;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<CartRepository> _logger;

    public CartRepository(ApplicationDbContext dbContext, IMapper mapper, ILogger<CartRepository> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<CartDto> GetCartByUserId(string userId)
    {
        try
        {
            var cart = new Cart()
            {
                CartHeader = await _dbContext.CartHeaders
                    .FirstOrDefaultAsync(ch => ch.UserId == userId)
            };

            cart.CartDetails = _dbContext.CartDetails
                .Where(cd => cd.CartHeaderId == cart.CartHeader.CartHeaderId)
                .Include(cd => cd.Product);

            return _mapper.Map<CartDto>(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError($"GetCartById method error for userId-{userId}.", ex);
            return new CartDto();
        }
    }

    public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
    {
        try
        {
            var cart = _mapper.Map<Cart>(cartDto);
            var prodIdDb =
                _dbContext.Products.FirstOrDefault(p => p.ProductId == cartDto.CartDetails.FirstOrDefault().ProductId);
            
            if (prodIdDb is null)
            {
                _dbContext.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _dbContext.SaveChangesAsync();
            }

            var cartHeaderFromDb =
                await _dbContext.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(ch => ch.UserId == cart.CartHeader.UserId);

            if (cartHeaderFromDb is null)
            {
                _dbContext.CartHeaders.Add(cart.CartHeader);
                await _dbContext.SaveChangesAsync();
                
                cart.CartDetails.FirstOrDefault().Product = null;
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb = await _dbContext.CartDetails.AsNoTracking()
                    .FirstOrDefaultAsync(cd =>
                    cd.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                    cd.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (cartDetailsFromDb is null)
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetailsFromDb.Count;
                    _dbContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
            }
            
            return _mapper.Map<CartDto>(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError("CreateUpdateCart method error.", ex);
            return new CartDto();
        }
        
    }

    public async Task<bool> RemoveFromCart(long cartDetailId)
    {
        try
        {
            var cartDetail = await _dbContext.CartDetails
                .FirstOrDefaultAsync(cd => cd.CartDetailId == cartDetailId);

            int totalCountOfCartItems =
                _dbContext.CartDetails.Count(cd => cd.CartHeaderId == cartDetail.CartHeaderId);

            _dbContext.CartDetails.Remove(cartDetail);
            if (totalCountOfCartItems == 1)
            {
                var cartHeaderToRemove =
                    _dbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.CartHeaderId == cartDetail.CartHeaderId);

                _dbContext.Remove(cartHeaderToRemove);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"RemoveFromCart method error for cartDetailId-{cartDetailId}.", ex);
            return false;
        }
    }

    public async Task<bool> ClearCart(string userId)
    {
        try
        {
            var cartHeaderFromDb = await _dbContext.CartHeaders.FirstOrDefaultAsync(ch => ch.UserId == userId);
            if (cartHeaderFromDb is not null)
            {
                _dbContext.CartDetails.RemoveRange(_dbContext.CartDetails
                    .Where(cd => cd.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                _dbContext.CartHeaders.Remove(cartHeaderFromDb);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ClearCart method error for userId-{userId}.", ex);
            return false;
        }
    }
}