using AutoMapper;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.ProductModels;

namespace Deppo.Mobile.Helpers.MappingHelper;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<ProductDetailModel, dynamic>();
		CreateMap<ProductTransaction, dynamic>();
	}
}
