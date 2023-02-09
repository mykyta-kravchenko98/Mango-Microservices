using Mango.Web;
using Mango.Web.Services;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy  =>
        {
            policy.WithOrigins("https://kit.fontawesome.com");
        });
});
// Add services to the container.
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddHttpClient<ICouponService, CouponService>();
SD.ProductApiBase = builder.Configuration["ServiceUrls:ProductApi"];
SD.ShoppingCartApiBase = builder.Configuration["ServiceUrls:ShoppingCartApi"];
SD.CouponApiBase = builder.Configuration["ServiceUrls:CouponApi"];

builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultScheme = "Cookies";
        opt.DefaultChallengeScheme = "oidc";
    }).AddCookie("Cookies", c => c.ExpireTimeSpan = TimeSpan.FromMinutes(10))
    .AddOpenIdConnect("oidc", opt =>
    {
        opt.Authority = builder.Configuration["ServiceUrls:IdentityApi"];
        opt.GetClaimsFromUserInfoEndpoint = true;
        opt.ClientId = "mango";
        opt.ClientSecret = "secret";
        opt.ResponseType = "code";

        opt.ClaimActions.MapJsonKey("role","role", "role");
        opt.ClaimActions.MapJsonKey("sub","sub", "sub");
        opt.TokenValidationParameters.NameClaimType = "name";
        opt.TokenValidationParameters.RoleClaimType = "role";
        opt.Scope.Add("mango");
        opt.SaveTokens = true;
    });
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();