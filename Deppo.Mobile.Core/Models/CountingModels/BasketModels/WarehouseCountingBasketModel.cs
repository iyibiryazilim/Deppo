using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.CountingModels.BasketModels;

public partial class WarehouseCountingBasketModel : ObservableObject
{

	[ObservableProperty]
	int itemReferenceId;

	[ObservableProperty]
	string itemCode = string.Empty;

	[ObservableProperty]
    string itemName = string.Empty;

	[ObservableProperty]
	int mainItemReferenceId;

	[ObservableProperty]
    string mainItemCode = string.Empty;

    [ObservableProperty]
    string mainItemName = string.Empty;


    [ObservableProperty]
	int subUnitsetReferenceId;

	[ObservableProperty]
	string subUnitsetName = string.Empty;

	[ObservableProperty]
	string subUnitsetCode = string.Empty;

	[ObservableProperty]
	int unitsetReferenceId;
	[ObservableProperty]
	string unitsetName = string.Empty;
	[ObservableProperty]
	string unitsetCode = string.Empty;

	[ObservableProperty]
	double stockQuantity;

	[ObservableProperty]
	double outputQuantity;

	[ObservableProperty]
	string? image;

	[ObservableProperty]
	bool isVariant;

	[ObservableProperty]
	int trackingType;

	[ObservableProperty]
	int locTracking;

	[ObservableProperty]
	ObservableCollection<LocationTransactionModel> locationTransactions = new();

	[ObservableProperty]
	double differenceQuantity = 0;

	private bool isCompleted;
	public bool IsCompleted
	{
		get => isCompleted;
		set
		{
			if (SetProperty(ref isCompleted, value))
			{
				OnPropertyChanged(nameof(CountingText));
				OnPropertyChanged(nameof(CountingTextColor));
			}
		}
	}

	private string countingText;
	public string CountingText
	{
		get => IsCompleted ? "Sayıldı" : "Bekliyor";
		set => SetProperty(ref countingText, value);
	}

	private string countingTextColor;

	public string CountingTextColor
	{
		get => IsCompleted ? "#008000" : "#E6BE0C";
		set => SetProperty(ref countingTextColor, value);
	}


}
