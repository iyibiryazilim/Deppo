using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using Org.Apache.Http.Impl.Client;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]

public partial class OutputProductSalesOrderProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly ISerilotTransactionService _serilotTransactionService;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	[ObservableProperty]
	ObservableCollection<OutputSalesBasketModel> items = null!;

	public OutputProductSalesOrderProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationTransactionService locationTransactionService, ISerilotTransactionService serilotTransactionService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationTransactionService = locationTransactionService;
		_serilotTransactionService = serilotTransactionService;

		Title = "Satış Sepeti";

		IncreaseCommand = new Command<OutputSalesBasketModel>(async (outputSalesBasketModel) => await IncreaseAsync(outputSalesBasketModel));
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());
	}
	public ContentPage CurrentPage { get; set; } = null!;

	public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

	#region Commands
	public Command<OutputSalesBasketModel> DeleteItemCommand { get; }
	public Command<OutputSalesBasketModel> IncreaseCommand {get; }
	public Command<OutputSalesBasketModel> DecreaseCommand { get; }

	public Command BackCommand { get; }
	public Command NextViewCommand { get; }
	public Command LocationTransactionConfirmCommand { get; }
	public Command LocationTransactionCloseCommand { get; }
	#endregion

	private async Task IncreaseAsync(OutputSalesBasketModel outputSalesBasketModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if(outputSalesBasketModel.LocTracking == 1)
			{
				await LoadWarehouseLocationTransactionsAsync(outputSalesBasketModel);
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				
			}
			else if(outputSalesBasketModel.TrackingType == 1)
			{
				CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
			}
			else
			{
				outputSalesBasketModel.OutputQuantity += 1;
			}
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadWarehouseLocationTransactionsAsync(OutputSalesBasketModel outputSalesBasketModel)
	{
		try
		{
			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);

			LocationTransactions.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetInputObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, outputSalesBasketModel.ItemReferenceId, warehouseNumber: WarehouseModel.Number, 0, 20, "");
			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
			}

			_userDialogs.HideHud();
		}
		catch(Exception ex) { 
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}


	private async Task LocationTransactionCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
		});
	}

}
