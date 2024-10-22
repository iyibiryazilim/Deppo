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

		}
	}
}
