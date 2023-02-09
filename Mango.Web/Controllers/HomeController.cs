using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Models.Dtos;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly IShoppingCartService _shoppingCartService;

    public HomeController(ILogger<HomeController> logger, IProductService productService, IShoppingCartService shoppingCartService)
    {
        _productService = productService;
        _logger = logger;
        _shoppingCartService = shoppingCartService;
    }

    public async Task<IActionResult> Index()
    {
        var products = new List<ProductDto>();
        var response = await _productService.GetAllProductsAsync<ResponseDto>();
        if (response is not null && response.IsSuccess)
        {
            products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
        }
        return View(products);
    }

    [Authorize]
    public async Task<IActionResult> Details(int productId)
    {
        var product = new ProductDto();
        var response = await _productService.GetProductsByIdAsync<ResponseDto>(productId);
        if (response is not null && response.IsSuccess)
        {
            product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
        }
        return View(product);
    }
    
    [HttpPost]
    [ActionName("Details")]
    [Authorize]
    public async Task<IActionResult> DetailsPost(ProductDto productDto)
    {
        var cartDto = new CartDto()
        {
            CartHeader = new CartHeaderDto()
            {
                UserId = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value
            }
        };

        var cartDetail = new CartDetailDto()
        {
            Count = productDto.Count,
            ProductId = productDto.ProductId
        };

        var resp = await _productService.GetProductsByIdAsync<ResponseDto>(productDto.ProductId);
        if (resp is not null && resp.IsSuccess)
        {
            cartDetail.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result));
        }

        var cardDetailDto = new List<CartDetailDto>();
        cardDetailDto.Add(cartDetail);
        cartDto.CartDetails = cardDetailDto;

        var addToCartResp = await _shoppingCartService.AddCartAsync<ResponseDto>(cartDto);
        if (addToCartResp is not null && addToCartResp.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }
        
        return View(productDto);
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [Authorize]
    public async Task<IActionResult> Login()
    {
        return RedirectToAction(nameof(Index));
    }
    public IActionResult Logout()
    {
        return SignOut("Cookies", "oidc");
    }
}