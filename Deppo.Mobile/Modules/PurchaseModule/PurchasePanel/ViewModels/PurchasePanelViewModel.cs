using System;
using System.Collections.ObjectModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchasePanel.ViewModels;

public partial class PurchasePanelViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IPurchasePanelService _purchasePanelService;
    public PurchasePanelViewModel( IHttpClientService httpClientService , IPurchasePanelService purchasePanelService)
    {
        
        _httpClientService = httpClientService;
        _purchasePanelService = purchasePanelService;
        Title = "Satınalma Paneli";
       
    }
    private int WaitingOrderCount;
    private int TotalOrderCount;
    private int ShippedOrderCount;
    private ObservableCollection<Supplier> LastCustomer = new ObservableCollection<Supplier>();
    private ObservableCollection<SupplierTransaction> LastCustomerTransaction = new ObservableCollection<SupplierTransaction>();
}
