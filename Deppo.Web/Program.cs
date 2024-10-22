using Deppo.Core.DataStores;
using Deppo.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("helix", c =>
{
	c.BaseAddress = new Uri("http://172.16.1.25:52789");
});

builder.Services.AddSingleton<IAuthenticationService, AuthenticateDataStore>();
builder.Services.AddTransient<IProductService, ProductDataStoreV2>();
builder.Services.AddTransient<ICustomerService, CustomerDataStoreV2>();
builder.Services.AddTransient<ISupplierService, SupplierDataStoreV2>();
builder.Services.AddTransient<IWarehouseService, WarehouseDataStore>();
builder.Services.AddTransient<IWaitingSalesOrderService, WaitingSalesOrderDataStore>();
builder.Services.AddTransient<IWaitingPurchaseOrderService, WaitingPurchaseOrderDataStore>();
builder.Services.AddTransient<IOutsourceService, OutsourceDataStore>();
builder.Services.AddTransient<INegativeProductService, NegativeProductDataStore>();



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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
