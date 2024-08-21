using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(Product), queryId: nameof(Product))]
public partial class ProductInputTransactionViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ICustomQueryService _customQueryService;
	private readonly IUserDialogs _userDialogs;
	public ProductInputTransactionViewModel(IHttpClientService httpClientService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
	{
		Title = "Malzeme Giriş Hareketleri";
		_httpClientService = httpClientService;
		_customQueryService = customQueryService;
		_userDialogs = userDialogs;

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		GoToBackCommand = new Command(async () => await GoToBackAsync());
	}

	#region Collections
	public ObservableCollection<ProductTransaction> Items { get; } = new();
	#endregion

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command GoToBackCommand { get; }
	#endregion

	#region Properties
	[ObservableProperty]
	Product product = null!;
	#endregion

	async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var query = @$"SELECT
		[ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
        [TransactionNumber] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [ProductReferenceId] = STLINE.STOCKREF,
        [ProductCode] = ITEMS.CODE,
        [ProductName] = ITEMS.NAME,
        [SubUnitsetCode] = SUBUNITSET.CODE,
        [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
        [Quantity] = STLINE.AMOUNT,
        [IOType] = STLINE.IOCODE,
		[UnitPrice] = STLINE.PRICE,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME
        FROM LG_001_02_STLINE AS STLINE
        LEFT JOIN LG_001_02_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_001_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_001_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_001_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF AND MAINUNIT = 1
        LEFT JOIN LG_001_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = 1
		WHERE STLINE.IOCODE IN (1,2) AND ITEMS.LOGICALREF = {Product.ReferenceId}";

			Items.Clear();

			_userDialogs.Loading("Loading Items...");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _customQueryService.GetObjectsAsync(httpClient, query);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<ProductTransaction>(item));
				}
				_userDialogs.Loading().Hide();
			}
			else
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.Loading().Hide();

				_userDialogs.Alert(message: result.Message, title: "Load Items");
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Load Items Error");
		}
		finally
		{
			IsBusy = false;
		}
	}

	async Task GoToBackAsync()
	{
		try
		{
			IsBusy = true;

			await Task.Delay(300);
			await Shell.Current.GoToAsync("..");
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
