using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ReworkModels.BasketModels;

/// <summary>
///  Used for ManuelRework
/// </summary>

public partial class ReworkBasketModel : ObservableObject
{
	[ObservableProperty]
	WarehouseModel? outWarehouseModel; // Çıkış ambarı

	[ObservableProperty]
	ReworkOutProductModel? reworkOutProductModel; // Sarf edilecek ürün

	[ObservableProperty]
	WarehouseModel? inWarehouseModel;  // Giriş Ambarı

	[ObservableProperty]
	ObservableCollection<ReworkInProductModel> reworkInProducts = new(); // Girdi ürünleri
}
