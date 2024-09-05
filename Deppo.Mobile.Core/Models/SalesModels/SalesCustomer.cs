using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.SalesModels;

public partial class SalesCustomer : ObservableObject
{
    [ObservableProperty]
    int referenceId;

    [ObservableProperty]
    string code = string.Empty;

    [ObservableProperty]
    string name = string.Empty;

    [ObservableProperty]
    int productReferenceCount;

    [ObservableProperty]
    string country = string.Empty;

    [ObservableProperty]
    string city = string.Empty;

	public string TitleName => Name?.Length > 2 ? Name.Substring(0, 2) : Name;

	[ObservableProperty]
    public List<SalesCustomerProduct> products = new();

    [ObservableProperty]
    bool isSelected;
}
