using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;


[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductProcessProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseTotalService _warehouseTotalService;
    private readonly IUserDialogs _userDialogs;
    public OutputProductProcessProductListViewModel(IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseTotalService = warehouseTotalService;
        _userDialogs = userDialogs;
		Title = "Ürün Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
    }

    #region Properties
    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    #endregion

    #region Commands
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command BackCommand { get; }
    #endregion

    #region Collections
    public ObservableCollection<WarehouseTotalModel> Items { get; } = new();
    #endregion

    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Loading Items...");
            Items.Clear();
            await Task.Delay(1000);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseTotalService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number);

            if(result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<WarehouseTotal>(product);

                    Items.Add(new WarehouseTotalModel
                    {
                        ProductReferenceId = item.ProductReferenceId,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        UnitsetReferenceId = item.UnitsetReferenceId,
                        UnitsetCode = item.UnitsetCode,
                        UnitsetName = item.UnitsetName,
                        SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                        SubUnitsetCode = item.SubUnitsetCode,
                        StockQuantity = item.StockQuantity,
                        WarehouseReferenceId = item.WarehouseReferenceId,
                        WarehouseName = item.WarehouseName,
                        WarehouseNumber = item.WarehouseNumber,
                        IsSelected = false,
                    });
                }
            }

            _userDialogs.Loading().Hide();
        }
        catch (Exception ex)
        {
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
        finally
        {
            IsBusy = false;
        }
    }
}
