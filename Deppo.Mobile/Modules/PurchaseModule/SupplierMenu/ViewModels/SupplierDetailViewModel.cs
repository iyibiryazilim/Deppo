using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.Views;

namespace Deppo.Mobile.Modules.PurchaseModule.SupplierMenu.ViewModels;

[QueryProperty(name: nameof(SupplierDetailModel), queryId: nameof(SupplierDetailModel))]
public partial class SupplierDetailViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ISupplierTransactionService _supplierTransactionService;
	private readonly ICustomQueryService _customQueryService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	private SupplierDetailModel supplierDetailModel = null!;

	public SupplierDetailViewModel(IHttpClientService httpClientService, ISupplierTransactionService supplierTransactionService, ICustomQueryService customQueryService, IUserDialogs userDialogs)
	{
		Title = "Tedarikçi Detayı";

		_httpClientService = httpClientService;
		_supplierTransactionService = supplierTransactionService;
		_customQueryService = customQueryService;
		_userDialogs = userDialogs;

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		InputQuantityTappedCommand = new Command(async () => await InputQuantityTappedAsync());
		OutputQuantityTappedCommand = new Command(async () => await OutputQuantityTappedAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command InputQuantityTappedCommand { get; }

	public Command OutputQuantityTappedCommand { get; }

	private async Task LoadItemsAsync()
	{
		try
		{
			IsBusy = true;


			_userDialogs.Loading("Loading Items...");
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			await Task.WhenAll(GetInputOutputQuantityAsync(httpClient), GetLastTransactionsAsync(httpClient));

			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Hata...");
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
                    [InputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_{_httpClientService.FirmNumber.ToString().PadLeft(3, '0')}_{_httpClientService.PeriodNumber.ToString().PadLeft(2, '0')}_STLINE WHERE IOCODE IN(1, 2) AND CLIENTREF = {SupplierDetailModel.Supplier.ReferenceId}),
                    [OutputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_{_httpClientService.FirmNumber.ToString().PadLeft(3, '0')}_{_httpClientService.PeriodNumber.ToString().PadLeft(2, '0')}_STLINE WHERE IOCODE IN(3, 4) AND CLIENTREF = {SupplierDetailModel.Supplier.ReferenceId})";

			var result = await _customQueryService.GetObjectAsync(httpClient, query);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;
				var obj = Mapping.Mapper.Map<SupplierDetailModel>(result.Data);
				SupplierDetailModel.InputQuantity = obj.InputQuantity;
				SupplierDetailModel.OutputQuantity = obj.OutputQuantity;
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			_userDialogs.Alert(message: ex.Message, title: "Hata");
		}
	}

	private async Task GetLastTransactionsAsync(HttpClient httpClient)
	{
		try
		{

			SupplierDetailModel.LastTransactions.Clear();

			var result = await _supplierTransactionService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				supplierReferenceId: SupplierDetailModel.Supplier.ReferenceId,
				skip: 0,
				take: 5
			);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var item in result.Data)
				{
					SupplierDetailModel.LastTransactions.Add(Mapping.Mapper.Map<SupplierTransaction>(item));
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
		try
		{
			IsBusy = true;

			await Task.Delay(300);
			await Shell.Current.GoToAsync($"{nameof(SupplierInputTransactionView)}", new Dictionary<string, object>
			{
				["Supplier"] = SupplierDetailModel.Supplier
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
		try
		{
			IsBusy = true;

			await Task.Delay(300);
			await Shell.Current.GoToAsync($"{nameof(SupplierOutputTransactionView)}", new Dictionary<string, object>
			{
				["Supplier"] = SupplierDetailModel.Supplier
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