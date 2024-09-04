using AutoMapper;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LoginModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;

namespace Deppo.Mobile.Helpers.MappingHelper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductDetailModel, dynamic>();
        CreateMap<ProductTransaction, dynamic>();
        CreateMap<WarehouseDetailModel, dynamic>();
        CreateMap<WarehouseTransaction, dynamic>();
        CreateMap<SupplierDetailModel, dynamic>();
        CreateMap<SupplierTransaction, dynamic>();
        CreateMap<CustomerDetailModel, dynamic>();
        CreateMap<CustomerTransaction, dynamic>();
        CreateMap<CompanyModel, dynamic>();
        CreateMap<Product, dynamic>();
        CreateMap<WarehouseTotal, dynamic>();
        CreateMap<Variant, dynamic>();
        CreateMap<Supplier, dynamic>();
        CreateMap<SupplierModel, dynamic>();
        CreateMap<CustomerModel, dynamic>();
        CreateMap<OutputProductBasketDetailModel, dynamic>();
        CreateMap<LocationTransaction, dynamic>();
        CreateMap<SerilotTransaction, dynamic>();
        CreateMap<SalesCustomer, dynamic>();
		CreateMap<SalesCustomerProduct, dynamic>();
		CreateMap<WaitingSalesOrder, dynamic>();
        CreateMap<PurchaseSupplier, dynamic>();
        CreateMap<PurchaseSupplierProduct, dynamic>();
        CreateMap<WaitingPurchaseOrderModel, dynamic>();
		CreateMap<WaitingSalesOrderModel, dynamic>();
	}
}