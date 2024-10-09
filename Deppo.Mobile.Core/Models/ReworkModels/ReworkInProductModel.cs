using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;

namespace Deppo.Mobile.Core.Models.ReworkModels;

public class ReworkInProductModel : ProductModel
{
	private double _inputQuantity;
	private WarehouseModel _inWarehouseModel;

	private List<ReworkInProductDetailModel> _details = new();

	public ReworkInProductModel()
	{
	}

	public double InputQuantity
	{
		get => _inputQuantity;
		set
		{
			if (_inputQuantity == value) return;
			_inputQuantity = value;
			NotifyPropertyChanged();
		}
	}

	public WarehouseModel InWarehouseModel
	{
		get => _inWarehouseModel;
		set
		{
			if (_inWarehouseModel == value) return;
			_inWarehouseModel = value;
			NotifyPropertyChanged();
		}
	}

	public List<ReworkInProductDetailModel> Details
	{
		get => _details;
		set
		{
			if (_details == value) return;
			_details = value;
			NotifyPropertyChanged();
		}
	}
}
