using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesCustomer : ObservableObject
{
    [ObservableProperty]
    private int referenceId;

    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private int productReferenceCount;

    [ObservableProperty]
    private string country = string.Empty;

    [ObservableProperty]
    private string city = string.Empty;

    [ObservableProperty]
    private int shipAddressReferenceId = 0;

    [ObservableProperty]
    private string shipAddressCode = string.Empty;
    [ObservableProperty]
    private string shipAddressName = string.Empty;

    [ObservableProperty]
    private int shipAddressCount;

    public string TitleName => Name?.Length > 2 ? Name.Substring(0, 2) : Name;

    [ObservableProperty]
    public List<SalesCustomerProduct> products = new();

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private int ficheType = default;

    [ObservableProperty]
    private bool isEDispatch;

    public string ShipAddressIcon => ShipAddressCount > 0 ? "truck" : "";
}