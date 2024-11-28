using Deppo.Core.DataStores;
using Deppo.Core.Services;
using Deppo.Sys.Service.DataStores;
using Deppo.Sys.Service.Services;
using Deppo.Web.DataStores;
using Deppo.Web.Models;
using Deppo.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("helix", (serviceProvider, c) =>
{
	

	var configuration = serviceProvider.GetRequiredService<IConfiguration>();
	var helixBaseAddress = configuration["HttpClientSettings:HelixBaseAddress"];
	c.BaseAddress = new Uri(helixBaseAddress);

	var cookieService = serviceProvider.GetRequiredService<ICookiePropertyService>();
	var cookieModel = cookieService.GetCookieModel();

	if (cookieModel != null && !string.IsNullOrEmpty(cookieModel.Token))
	{
		c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cookieModel.Token);
	}
});

builder.Services.AddHttpClient("sys", (serviceProvider, c) =>
{

	var configuration = serviceProvider.GetRequiredService<IConfiguration>();
	var sysBaseAddress = configuration["HttpClientSettings:SysBaseAddress"];
	c.BaseAddress = new Uri(sysBaseAddress);

	var cookieService = serviceProvider.GetRequiredService<ICookiePropertyService>();
	var cookieModel = cookieService.GetCookieModel();


	if (cookieModel != null && !string.IsNullOrEmpty(cookieModel.Token))
	{
		c.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cookieModel.Token);
	}
});


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ICookiePropertyService, CookiePropertyDataStore>();

builder.Services.AddSingleton<IAuthenticationService, AuthenticateDataStore>();
builder.Services.AddTransient<IProductService, ProductDataStoreV2>();
builder.Services.AddTransient<IProductDetailService, ProductDetailDataStore>();
builder.Services.AddTransient<IProductDetailActionService, ProductDetailActionDataStore>();
builder.Services.AddTransient<IVariantService, VariantDataStore>();
builder.Services.AddTransient<ICustomerService, CustomerDataStoreV2>();
builder.Services.AddTransient<ISupplierService, SupplierDataStoreV2>();
builder.Services.AddTransient<Deppo.Core.Services.IWarehouseService, Deppo.Core.DataStores.WarehouseDataStore>();
builder.Services.AddTransient<IWaitingSalesOrderService, WaitingSalesOrderDataStore>();
builder.Services.AddTransient<IWaitingPurchaseOrderService, WaitingPurchaseOrderDataStore>();
builder.Services.AddTransient<IOutsourceService, OutsourceDataStore>();
builder.Services.AddTransient<INegativeProductService, NegativeProductDataStore>();
builder.Services.AddTransient<IQuicklyBomService, QuicklyBomDataStore>();

#region Sys Service
builder.Services.AddTransient<IAuthenticateSysService, AuthenticateSysDataStore>();
builder.Services.AddTransient<IApplicationUserService, ApplicationUserDataStore>(); 
builder.Services.AddTransient<ITransactionAuditService, TransactionAuditDataStore>();
builder.Services.AddTransient<IMediaDataObjectService, MediaDataObjectDataStore>();
builder.Services.AddTransient<IReasonsForRejectionProcurementService, ReasonsForRejectionProcurementDataStore>();
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
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);

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
