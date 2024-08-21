using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.WarehouseMenu.Views;

namespace Deppo.Mobile.Modules.ProductModule.WarehouseMenu.ViewModels;

[QueryProperty(name: nameof(WarehouseDetailModel), queryId: nameof(WarehouseDetailModel))]
public partial class WarehouseDetailViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly ICustomQueryService _customQueryService;
    private readonly IUserDialogs _userDialogs;

    public WarehouseDetailViewModel(IHttpClientService httpClientService, IWarehouseService warehouseService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
    {
        Title = "Ambar Detayı";
        _httpClientService = httpClientService;
        _warehouseService = warehouseService;
        _customQueryService = customQueryService;
        _userDialogs = userDialogs;

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        InputQuantityTappedCommand = new Command(async () => await InputQuantityTappedAsync());
        OutputQuantityTappedCommand = new Command(async () => await OutputQuantityTappedAsync());

    }

    [ObservableProperty]
    WarehouseDetailModel warehouseDetailModel = null!;

    #region Commands
    public Command LoadItemsCommand { get; }
    public Command InputQuantityTappedCommand { get; }
    public Command OutputQuantityTappedCommand { get; }

    #endregion Commands

    private async Task LoadItemsAsync()
    {
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            _userDialogs.Loading("Loading Items...");

            await Task.Delay(1000);
            await Task.WhenAll(GetInputOutputQuantityAsync(httpClient), GetLastTransactionsAsync(httpClient));

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetInputOutputQuantityAsync(HttpClient httpClient)
    {
        try
        {
            var query = @$"SELECT 
                [InputQuantity] = COUNT(CASE WHEN STLINE.IOCODE IN (1, 2) THEN STLINE.LOGICALREF END),
                [OutputQuantity] = COUNT(CASE WHEN STLINE.IOCODE IN (3, 4) THEN STLINE.LOGICALREF END)
            FROM LG_001_02_STLINE AS STLINE
            LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE 
            ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = 1
            WHERE STLINE.SOURCEINDEX = {WarehouseDetailModel.Warehouse.Number};
            ";

            var result = await _customQueryService.GetObjectAsync(httpClient, query);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;
                var obj = Mapping.Mapper.Map<WarehouseDetailModel>(result.Data);
                WarehouseDetailModel.InputQuantity = obj.InputQuantity;
                WarehouseDetailModel.OutputQuantity = obj.OutputQuantity;
            }
		}
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
    }

    private async Task GetLastTransactionsAsync(HttpClient httpclient)
    {
        try
        {
            WarehouseDetailModel.LastTransactions.Clear();

            var query = @$"SELECT TOP 5
				[TransactionDate] = STLINE.DATE_,
				[TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
				[TransactionNumber] = STFICHE.FICHENO,
				[TransactionType] = STLINE.TRCODE,
                [IOType] = STLINE.IOCODE,
				[SubUnitsetCode] = SUBUNITSET.CODE,
				[Quantity] = STLINE.AMOUNT,
				[WarehouseName] = CAPIWHOUSE.NAME
				FROM LG_001_02_STLINE AS STLINE
				LEFT JOIN LG_001_02_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
				LEFT JOIN LG_001_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
				LEFT JOIN LG_001_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF AND MAINUNIT = 1
				LEFT JOIN LG_001_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
				LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = 1
				WHERE CAPIWHOUSE.LOGICALREF= {WarehouseDetailModel.Warehouse.ReferenceId} ORDER BY STLINE.DATE_ DESC";

            var result = await _customQueryService.GetObjectsAsync(httpclient, query);

            if (result.IsSuccess)
            {
                if (result.Data == null)
                    return;

                foreach (var item in result.Data)
                {
                    WarehouseDetailModel.LastTransactions.Add(Mapping.Mapper.Map<WarehouseTransaction>(item));
                }
            }
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

            _userDialogs.Alert(message: ex.Message, title: "Hata");
        }
    }

	private async Task InputQuantityTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Task.Delay(300);
			await Shell.Current.GoToAsync($"{nameof(WarehouseInputTransactionView)}", new Dictionary<string, object>
			{
				["Warehouse"] = WarehouseDetailModel.Warehouse
			});
		}
		catch (Exception ex)
		{

			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Hata");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task OutputQuantityTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(WarehouseOutputTransactionView)}", new Dictionary<string, object>
            {
                ["Warehouse"] = WarehouseDetailModel.Warehouse
            });
        }
        catch (Exception ex)
        {

			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Hata");
		}
        finally
        {
            IsBusy = false;
        }
    }
}