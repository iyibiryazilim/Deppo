using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SeriLotModels;

namespace Deppo.Mobile.Core.Models.TransferModels;

public class OutProductModel : ProductModel
{
    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; set; } = new ObservableCollection<LocationTransactionModel>();

    private double _outputQuantity;

	public double OutputQuantity
	{
		get => _outputQuantity;
		set
		{
			if (_outputQuantity == value) return;
			_outputQuantity = value;
			NotifyPropertyChanged();
		}
	}
}
