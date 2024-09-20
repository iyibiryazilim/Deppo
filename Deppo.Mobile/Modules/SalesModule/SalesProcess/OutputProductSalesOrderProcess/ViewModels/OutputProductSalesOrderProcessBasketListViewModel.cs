using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
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
	private readonly ISeriLotTransactionService _seriLotTransactionService;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	[ObservableProperty]
	ObservableCollection<OutputSalesBasketModel> items = null!;

	[ObservableProperty]
	OutputSalesBasketModel? selectedItem;

	[ObservableProperty]
	LocationTransactionModel? selectedLocationTransaction;
	[ObservableProperty]
	SeriLotTransactionModel? selectedSeriLotTransaction;


	[ObservableProperty]
	public ObservableCollection<LocationTransactionModel> selectedLocationTransactions = new();
	[ObservableProperty]
	public ObservableCollection<SeriLotTransactionModel> selectedSeriLotTransactions = new();

	public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();
	public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();

	public OutputProductSalesOrderProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationTransactionService locationTransactionService, ISeriLotTransactionService seriLotTransactionService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationTransactionService = locationTransactionService;
		_seriLotTransactionService = seriLotTransactionService;

		Title = "Satış Sepeti";

		IncreaseCommand = new Command<OutputSalesBasketModel>(async (outputSalesBasketModel) => await IncreaseAsync(outputSalesBasketModel));
		DecreaseCommand = new Command<OutputSalesBasketModel>(async (outputSalesBasketModel) => await DecreaseAsync(outputSalesBasketModel));
		BackCommand = new Command(async () => await BackAsync());
		
		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreWarehouseLocationTransactionsAsync());
		LocationTransactionIncreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
		LocationTransactionDecreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
		LocationTransactionConfirmCommand = new Command(ConfirmLocationTransactionAsync);
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

		LoadMoreSeriLotTransactionsCommand = new Command(async () => await LoadMoreSeriLotTransactionsAsync());
		SeriLotTransactionIncreaseCommand = new Command<SeriLotTransactionModel>(async (item) => SeriLotTransactionIncreaseAsync(item));
		SeriLotTransactionDecreaseCommand = new Command<SeriLotTransactionModel>(async (item) => SeriLotTransactionDecreaseAsync(item));
		SeriLotTransactionConfirmCommand = new Command(ConfirmSeriLotTransactionAsync);
		SeriLotTransactionCloseCommand = new Command(async () => await SeriLotTransactionCloseAsync());
	}
	public ContentPage CurrentPage { get; set; } = null!;

	#region Commands
	public Command<OutputSalesBasketModel> IncreaseCommand {get; }
	public Command<OutputSalesBasketModel> DecreaseCommand { get; }
	public Command<OutputSalesBasketModel> DeleteItemCommand { get; }

	#region LocationTransaction Command
	public Command LoadMoreLocationTransactionsCommand { get; }
	public Command LocationTransactionIncreaseCommand { get; }
	public Command LocationTransactionDecreaseCommand { get; }
	public Command LocationTransactionConfirmCommand { get; }
	public Command LocationTransactionCloseCommand { get; }
	#endregion

	#region SeriLotTransaction Command
	public Command LoadMoreSeriLotTransactionsCommand { get; }   
	public Command SeriLotTransactionIncreaseCommand { get; }
	public Command SeriLotTransactionDecreaseCommand { get; }
	public Command SeriLotTransactionConfirmCommand { get; }
	public Command SeriLotTransactionCloseCommand { get; }
	#endregion


	public Command BackCommand { get; }
	public Command NextViewCommand { get; }
	#endregion

	private async Task IncreaseAsync(OutputSalesBasketModel outputSalesBasketModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectedItem = outputSalesBasketModel;
			// Stok Yeri Takipli olma durumu
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
				if(outputSalesBasketModel.OutputQuantity < outputSalesBasketModel.StockQuantity)
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

	private async Task DecreaseAsync(OutputSalesBasketModel outputSalesBasketModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if(outputSalesBasketModel is not null)
			{
				if(outputSalesBasketModel.OutputQuantity > 1)
				{
					// Stok Yeri Takipli olma durumu
					if (outputSalesBasketModel.LocTracking == 1)
					{
						await LoadWarehouseLocationTransactionsAsync(outputSalesBasketModel);
						CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
					}
					// SeriLot takipli olma durumu
					else if (outputSalesBasketModel.LocTracking == 0 && (outputSalesBasketModel.TrackingType == 1 || outputSalesBasketModel.TrackingType == 2))
					{
						await LoadSeriLotTransactionsAsync();
						CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
					}
					else
					{
							outputSalesBasketModel.OutputQuantity -= 1;
					}
				}
				
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
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

	private async Task LoadMoreWarehouseLocationTransactionsAsync()
	{
		if (IsBusy)
			return;
		if (LocationTransactions.Count < (18))  // 18 = Take (20) - Remaining ItemsThreshold (2)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetInputObjectsAsync(httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedItem.ItemReferenceId,
				warehouseNumber: WarehouseModel.Number,
				skip: LocationTransactions.Count,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
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

	private async Task LocationTransactionIncreaseAsync(LocationTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(item is not null)
			{
				SelectedLocationTransaction = item;
				if(SelectedItem.TrackingType == 1 ||SelectedItem.TrackingType == 2)
				{
					await LoadSeriLotTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				else
				{
                    var totalQuantity = LocationTransactions.Sum(x => x.OutputQuantity);
                    if (SelectedItem.StockQuantity > totalQuantity)
                    {
                        if (item.OutputQuantity < item.RemainingQuantity && SelectedItem.StockQuantity > item.OutputQuantity)
                            item.OutputQuantity++;
                    }

                    if (item.OutputQuantity > 0 && !item.IsSelected)
						item.IsSelected = true;
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LocationTransactionDecreaseAsync(LocationTransactionModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item is not null)
			{
				// SeriLot takipli ise serilotTransactionBottomSheet aç
				if (SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2)
				{
					await LoadSeriLotTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				// SeriLot takipli değilse
				else
				{
					if (item.OutputQuantity > 0)
						item.OutputQuantity--;

					if(item.OutputQuantity == 0)
						item.IsSelected = false;
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void ConfirmLocationTransactionAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(LocationTransactions.Count > 0)
			{
				SelectedLocationTransactions.Clear();
				SelectedLocationTransactions.ToList().AddRange(LocationTransactions.Where(x => x.OutputQuantity > 0));

                foreach (var item in SelectedLocationTransactions)
                {
					var selectedLocationTransactionItem = SelectedItem.Details.FirstOrDefault(x => x.TransactionReferenceId == item.TransactionReferenceId);
					if (selectedLocationTransactionItem is not null)
					{
						selectedLocationTransactionItem.Quantity = item.OutputQuantity;
					}

					SelectedItem.Details.Add(new OutputSalesBasketDetailModel
					{
						LocationReferenceId = item.LocationReferenceId,
						LocationCode = item.LocationCode,
						LocationName = item.LocationName,
						Quantity = item.OutputQuantity,
						TransactionReferenceId = item.TransactionReferenceId,
						TransactionFicheReferenceId = item.TransactionFicheReferenceId,
						InTransactionReferenceId = item.InTransactionReferenceId,
						RemainingQuantity = item.RemainingQuantity,
						RemainingUnitQuantity = item.RemainingUnitQuantity,
					});
                }


                var totalOutputQuantity = SelectedLocationTransactions.Sum(x => (double)x.OutputQuantity);
				SelectedItem.OutputQuantity = totalOutputQuantity;

				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
			}
			else
			{
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
			}

		}
		catch (Exception ex)
		{

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LocationTransactionCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	private async Task LoadSeriLotTransactionsAsync()
	{
		try
		{
			_userDialogs.ShowLoading("Load Serilot Items...");
			await Task.Delay(1000);
			SeriLotTransactions.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _seriLotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, search: string.Empty);

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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadMoreSeriLotTransactionsAsync()
	{
		if (IsBusy)
			return;
		if (SeriLotTransactions.Count < 18)
			return;

		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _seriLotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, skip: SeriLotTransactions.Count, take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					SeriLotTransactions.Add(Mapping.Mapper.Map<SeriLotTransactionModel>(item));
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void SeriLotTransactionIncreaseAsync(SeriLotTransactionModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if(item is not null)
			{
				if(item.OutputQuantity < item.Quantity)
				{
					item.OutputQuantity++;

					if (item.OutputQuantity > 0 && !item.IsSelected)
						item.IsSelected = true;
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void SeriLotTransactionDecreaseAsync(SeriLotTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item is not null)
			{
				if (item.OutputQuantity == 0)
					item.IsSelected = false;
				if (item.OutputQuantity > 0)
				{
					item.OutputQuantity--;
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void ConfirmSeriLotTransactionAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(SeriLotTransactions.Count > 0)
			{
				SelectedSeriLotTransactions.Clear();
				SelectedSeriLotTransactions.ToList().AddRange(SeriLotTransactions.Where(x => x.OutputQuantity > 0));
				// Stok yeri takipli değilse
				if(SelectedItem.LocTracking == 0)
				{
					var totalOutputQuantity = SeriLotTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
					SelectedItem.OutputQuantity = totalOutputQuantity;

					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
				}
				// Stok yeri takipli ise
				else
				{	
					var totalOutputQuantity = SeriLotTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
					SelectedLocationTransaction.OutputQuantity = totalOutputQuantity;
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
				}
			}
			CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
		}
		catch(Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}


	private async Task SeriLotTransactionCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (Items.Count > 0)
			{
				var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
				if (!result)
					return;

				SelectedLocationTransactions.Clear();
				SelectedSeriLotTransactions.Clear();
				Items.Clear();
				await Shell.Current.GoToAsync("..");
			}
			else
			{
				await Shell.Current.GoToAsync("..");
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
}
