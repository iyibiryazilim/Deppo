using AutoMapper;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;

namespace Deppo.Web.Helpers.MappingHelper
{
	public class MappingProfile : Profile
	{
        public MappingProfile()
        {
			CreateMap<Product, dynamic>();
			CreateMap<Customer, dynamic>();
			CreateMap<Supplier, dynamic>();
			CreateMap<Warehouse, dynamic>();
            CreateMap<WaitingSalesOrder, dynamic>();
            CreateMap<WaitingPurchaseOrder, dynamic>();
            CreateMap<Outsource, dynamic>();
			CreateMap<NegativeProduct, dynamic>();





		}
	}
}
