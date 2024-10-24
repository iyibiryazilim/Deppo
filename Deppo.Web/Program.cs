using Deppo.Core.DataStores;
using Deppo.Core.Services;
using Deppo.Sys.Service.DataStores;
using Deppo.Sys.Service.Services;
using Deppo.Web.DataStores;
using Deppo.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("helix", c =>
{
	c.BaseAddress = new Uri("http://172.16.1.25:52789");
});
builder.Services.AddHttpClient("sys", c =>
{
	c.BaseAddress = new Uri("http://172.16.1.3:1923");
});


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ICookiePropertyService, CookiePropertyDataStore>();

builder.Services.AddSingleton<IAuthenticationService, AuthenticateDataStore>();
builder.Services.AddTransient<IProductService, ProductDataStoreV2>();
builder.Services.AddTransient<ICustomerService, CustomerDataStoreV2>();
builder.Services.AddTransient<ISupplierService, SupplierDataStoreV2>();
builder.Services.AddTransient<Deppo.Core.Services.IWarehouseService, Deppo.Core.DataStores.WarehouseDataStore>();
builder.Services.AddTransient<IWaitingSalesOrderService, WaitingSalesOrderDataStore>();
builder.Services.AddTransient<IWaitingPurchaseOrderService, WaitingPurchaseOrderDataStore>();
builder.Services.AddTransient<IOutsourceService, OutsourceDataStore>();
builder.Services.AddTransient<INegativeProductService, NegativeProductDataStore>();

#region Sys Service
builder.Services.AddTransient<IAuthenticateSysService, AuthenticateSysDataStore>();
builder.Services.AddTransient<IApplicationUserService, ApplicationUserDataStore>(); 
builder.Services.AddTransient<ITransactionAuditService, TransactionAuditDataStore>();
#endregion

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddMvcCore();
//builder.Services.AddAuthorization(x => x.AddPolicy("UserClaimAdministratorPolicy", policy => policy.RequireClaim("Administrator", "Administrator")));
//builder.Services.AddAuthorization(x => x.AddPolicy("UserClaimUserPolicy", policy => policy.RequireClaim("User", "User")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Authentication/Index/";
        option.LogoutPath = "/Authentication/Index/";
        //option.AccessDeniedPath = "/Authentication/AccessDenied/";
        option.SlidingExpiration = true;
        option.ExpireTimeSpan = TimeSpan.FromMinutes(60);

    });


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

app.UseAuthorization();
app.UseAuthentication();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
