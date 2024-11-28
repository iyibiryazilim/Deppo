using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByProductModels;

public class ProcurementProductBasketModel : INotifyPropertyChanged, IDisposable
{
	private WarehouseModel _orderWarehouse;
	private WarehouseModel _procurementWarehouse;
	private ObservableCollection<CustomerOrderModel> _selectedCustomers = new();
	private List<ProcurementProductBasketProductModel> _basketProducts = new();
	private List<ProcurementProductProcurableProductModel> _procurementProductList = new();

	private int _locationReferenceId;
	private string _locationCode = string.Empty;
	private string _locationName = string.Empty;

	public ProcurementProductBasketModel()
    {
        
    }

	[Browsable(false)]
	public int LocationReferenceId
	{
		get => _locationReferenceId;
		set
		{
			if (_locationReferenceId == value) return;
			_locationReferenceId = value;
			NotifyPropertyChanged();
		}
	}

	[DisplayName("Stok Yeri Kodu")]
	public string LocationCode
	{
		get => _locationCode;
		set
		{
			if (_locationCode == value) return;
			_locationCode = value;
			NotifyPropertyChanged();
		}
	}

	[DisplayName("Stok Yeri Adı")]
	public string LocationName
	{
		get => _locationName;
		set
		{
			if (_locationName == value) return;
			_locationName = value;
			NotifyPropertyChanged();
		}
	}


	[DisplayName("Sipariş Ambarı")]
	public WarehouseModel OrderWarehouse
	{
		get => _orderWarehouse;
		set
		{
			if (_orderWarehouse == value) return;
			_orderWarehouse = value;
			NotifyPropertyChanged();
		}
	}

	[DisplayName("Ürün Toplama Ambarı")]
	public WarehouseModel ProcurementWarehouse
	{
		get => _procurementWarehouse;
		set
		{
			if (_procurementWarehouse == value) return;
			_procurementWarehouse = value;
			NotifyPropertyChanged();
		}
	}

	[DisplayName("Seçilen Müşteriler")]
	public ObservableCollection<CustomerOrderModel> SelectedCustomers
	{
		get => _selectedCustomers;
		set
		{
			if (_selectedCustomers == value) return;
			_selectedCustomers = value;
			NotifyPropertyChanged();
		}
	}

	public List<ProcurementProductBasketProductModel> BasketProducts
	{
		get => _basketProducts;
		set
		{
			if (_basketProducts == value) return;
			_basketProducts = value;
			NotifyPropertyChanged();
		}
	}

	
	public List<ProcurementProductProcurableProductModel> ProcurementProductList
	{
		get => _procurementProductList;
		set
		{
			if (_procurementProductList == value) return;
			_procurementProductList = value;
			NotifyPropertyChanged();
		}
	}



	public event PropertyChangedEventHandler? PropertyChanged;

	protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);

	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			PropertyChanged = null;
		}
	}
}
