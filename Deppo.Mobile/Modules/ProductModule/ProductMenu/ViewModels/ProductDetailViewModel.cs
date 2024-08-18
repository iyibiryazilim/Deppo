using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ProductModule.ProductMenu.ViewModels;

[QueryProperty(name: nameof(ProductDetailModel), queryId: nameof(ProductDetailModel))]
public partial class ProductDetailViewModel : BaseViewModel
{

	private readonly IHttpClientService _httpClientService;
	private readonly ICustomQueryService _customQueryService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	ProductDetailModel productDetailModel = null!;

	public ProductDetailViewModel(IHttpClientService httpClientService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
	{
		Title = "Ürün Detayı";
		_httpClientService = httpClientService;
		_customQueryService = customQueryService;
		_userDialogs = userDialogs;

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
	}

	public Command LoadItemsCommand { get; }

	async Task LoadItemsAsync()
	{
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			await Task.Delay(1000);

			// Querylerde yer alan firma numarasi dinamik olarak alinacak
			await Task.WhenAll(GetInputOutputQuantityAsync(httpClient), GetLastTransactionsAsync(httpClient));

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Hata");
		}
		finally
		{
			Console.WriteLine(ProductDetailModel);
			IsBusy = false;
		}
	}

	async Task GetInputOutputQuantityAsync(HttpClient httpClient)
	{
		try
		{
			var query = @$"SELECT 
                    [InputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_001_01_STLINE WHERE IOCODE IN(1, 2) AND STOCKREF = {ProductDetailModel.Product.ReferenceId}),
                    [OutputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_001_01_STLINE WHERE IOCODE IN(3, 4) AND STOCKREF = {ProductDetailModel.Product.ReferenceId})";

			var result = await _customQueryService.GetObjectAsync(httpClient, query);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;
				var obj = Mapping.Mapper.Map<ProductDetailModel>(result.Data);
				ProductDetailModel.InputQuantity = obj.InputQuantity;
				ProductDetailModel.OutputQuantity = obj.OutputQuantity;
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Hata");
		}
	}

	async Task GetLastTransactionsAsync(HttpClient httpclient)
	{
		try
		{
			var query = @$"SELECT TOP 5
				[TransactionDate] = STLINE.DATE_,
				[TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
				[BaseTransactionCode] = STFICHE.FICHENO,
				[TransactionType] = STLINE.TRCODE,
				[SubUnitsetCode] = SUBUNITSET.CODE,
				[SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
				[UnitsetCode] = UNITSET.CODE,
				[UnitsetReferenceId] = UNITSET.LOGICALREF,
				[Quantity] = STLINE.AMOUNT,
				[WarehouseName] = CAPIWHOUSE.NAME
				FROM LG_001_01_STLINE AS STLINE
				LEFT JOIN LG_001_01_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
				LEFT JOIN LG_001_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
				LEFT JOIN LG_001_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF AND MAINUNIT = 1
				LEFT JOIN LG_001_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
				LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = 1
				WHERE ITEMS.LOGICALREF= 19277 ORDER BY STLINE.DATE_ DESC";

			var result = await _customQueryService.GetObjectsAsync(httpclient, query);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var item in result.Data)
				{
					ProductDetailModel.LastTransactions.Add(Mapping.Mapper.Map<ProductTransaction>(item));
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
}