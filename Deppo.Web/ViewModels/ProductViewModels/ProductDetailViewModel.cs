using Deppo.Core.BaseModels;

namespace Deppo.Web.ViewModels.ProductViewModels
{
	public class ProductDetailViewModel
	{
		public Product Product { get; set; } = null!;

		public double OutputQuantity { get; set; }

		public double InputQuantity { get; set; }
	}
}
