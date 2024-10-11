using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.LocationModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.CountingModels.BasketModels;

public partial class ProductCountingBasketModel : ObservableObject
{
    [ObservableProperty]
    private int productReferenceId;

    [ObservableProperty]
    private string productCode = string.Empty;

    [ObservableProperty]
    private string productName = string.Empty;

    [ObservableProperty]
    private int subUnitsetReferenceId;

    [ObservableProperty]
    private string subUnitsetName = string.Empty;

    [ObservableProperty]
    private string subUnitsetCode = string.Empty;

    [ObservableProperty]
    private int unitsetReferenceId;

    [ObservableProperty]
    private string unitsetName = string.Empty;

    [ObservableProperty]
    private string unitsetCode = string.Empty;

    [ObservableProperty]
    private double stockQuantity;

    [ObservableProperty]
    private double outputQuantity;

    [ObservableProperty]
    private byte[]? image;

    [ObservableProperty]
    private bool isVariant;

    [ObservableProperty]
    private int trackingType;

    [ObservableProperty]
    private int locTracking;

    [ObservableProperty]
    private ObservableCollection<LocationTransactionModel> locationTransactions = new();

    [ObservableProperty]
    private double differenceQuantity = 0;
}