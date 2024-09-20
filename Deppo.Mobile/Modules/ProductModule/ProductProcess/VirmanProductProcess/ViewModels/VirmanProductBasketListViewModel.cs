using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.VirmanModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.ViewModels;

[QueryProperty(name: nameof(VirmanBasketModel), queryId: nameof(VirmanBasketModel))]
public partial class VirmanProductBasketListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISeriLotTransactionService _serilotTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IProductService _productService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILocationService _locationService;



    [ObservableProperty]
    private VirmanBasketModel virmanBasketModel= null!;

    public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();
	public ObservableCollection<SeriLotTransactionModel> selectedSeriLotTransactions = new();


    public VirmanProductBasketListViewModel(IHttpClientService httpClientService
        , ISeriLotTransactionService serilotTransactionService
        , IUserDialogs userDialogs
        , IProductService productService
        , IServiceProvider serviceProvider,
ILocationService locationService)
    {
        _httpClientService = httpClientService;
        _serilotTransactionService = serilotTransactionService;
        _userDialogs = userDialogs;
        _productService = productService;
        Title = "Virman Sepeti";
        _serviceProvider = serviceProvider;
        _locationService = locationService;
    }
    public Page CurrentPage { get; set; } = null!;

    public Command ShowProductViewCommand { get; }
    public Command<ReturnSalesBasketModel> DeleteItemCommand { get; }
    public Command<ReturnSalesBasketModel> IncreaseCommand { get; }
    public Command<ReturnSalesBasketModel> DecreaseCommand { get; }


    public ObservableCollection<LocationModel> Locations { get; }
    public Command<LocationModel> LocationDecreaseCommand { get; }
    public Command<LocationModel> LocationIncreaseCommand { get; }
    public Command<LocationModel> LocationConfirmCommand { get; }








    private async Task DecreaseAsync(VirmanBasketModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (item is not null)
            {
                if (item.Quantity > 1)
                {
                    // Stok Yeri takipli ise locationTransactionBottomSheet aç
                    if (item.LocTracking == 1)
                    {
                        await LoadLocationTransactionsAsync();
                        CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                    }
                    // Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
                    else if (item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
                    {
                        await LoadSeriLotTransactionsAsync();
                        CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                    }
                    // Stok yeri ve SeriLot takipli değilse
                    else
                    {
                        item.Quantity--;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }


    private async Task LoadSeriLotTransactionsAsync()
    {

        try
        {
            _userDialogs.ShowLoading("Load Serilot Items...");
            await Task.Delay(1000);
            SeriLotTransactions.Clear();
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: VirmanBasketModel.OutVirmanProduct.ReferenceId, warehouseNumber: VirmanBasketModel.OutVirmanWarehouse.Number, search: string.Empty);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    SeriLotTransactions.Add(Mapping.Mapper.Map<SeriLotTransactionModel>(item));
                }
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

}
