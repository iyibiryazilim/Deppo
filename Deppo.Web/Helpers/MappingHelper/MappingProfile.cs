using AutoMapper;
using Deppo.Core.BaseModels;

namespace Deppo.Web.Helpers.MappingHelper
{
	public class MappingProfile : Profile
	{
        public MappingProfile()
        {
			CreateMap<Product, dynamic>();
		}
    }
}
