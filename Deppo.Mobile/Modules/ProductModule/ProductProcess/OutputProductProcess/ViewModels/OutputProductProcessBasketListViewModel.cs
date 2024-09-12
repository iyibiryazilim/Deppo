using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

[QueryProperty(name: nameof(OutputProductProcessType), queryId: nameof(OutputProductProcessType))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ISeriLotTransactionService _serilotTransactionService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	OutputProductProcessType outputProductProcessType;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	OutputProductBasketModel? selectedItem;

	[ObservableProperty]
	SeriLotTransactionModel? selectedSeriLotTransaction;

	[ObservableProperty]
	LocationTransactionModel? selectedLocationTransaction;

	[ObservableProperty]
	public ObservableCollection<SeriLotTransactionModel> selectedSeriLotTransactions = new();

	[ObservableProperty]
	public ObservableCollection<LocationTransactionModel> selectedLocationTransactions = new();

	#region Collections
	public ObservableCollection<OutputProductBasketModel> Items { get; } = new();
	public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();
	public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();
	#endregion

	public OutputProductProcessBasketListViewModel(IHttpClientService httpClientService, ISeriLotTransactionService serilotTransactionService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_serilotTransactionService = serilotTransactionService;
		_locationTransactionService = locationTransactionService;
		_userDialogs = userDialogs;

		Title = "Sepet Listesi";

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		IncreaseCommand = new Command<OutputProductBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<OutputProductBasketModel>(async (item) => await DecreaseAsync(item));
		DeleteItemCommand = new Command<OutputProductBasketModel>(async (item) => await DeleteItemAsync(item));

		
		LoadMoreSeriLotTransactionsCommand = new Command(async () => await LoadMoreSeriLotTransactionsAsync());
		SeriLotTransactionIncreaseCommand = new Command<SeriLotTransactionModel>(item => SeriLotTransactionIncreaseAsync(item));
		SeriLotTransactionDecreaseCommand = new Command<SeriLotTransactionModel>(item => SeriLotTransactionDecreaseAsync(item));
		ConfirmSeriLotTransactionCommand = new Command(ConfirmSeriLotTransactionAsync);
		SeriLotTransactionCloseCommand = new Command(async () => await SeriLotTransactionCloseAsync());

		
		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
		LocationTransactionIncreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
		LocationTransactionDecreaseCommand = new Command<LocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
		ConfirmLocationTransactionCommand = new Command(ConfirmLocationTransactionAsync);
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	#region Commands
	public Command ShowProductViewCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command DeleteItemCommand { get; }

	#region Location Transaction Command
	public Command LoadMoreLocationTransactionsCommand { get; }
	public Command LocationTransactionIncreaseCommand { get; }
	public Command LocationTransactionDecreaseCommand { get; }
	public Command ConfirmLocationTransactionCommand { get; }
	public Command LocationTransactionCloseCommand { get; }
	#endregion

	#region SeriLotTransaction Command
	public Command LoadMoreSeriLotTransactionsCommand { get; }
	public Command SeriLotTransactionIncreaseCommand { get; }
	public Command SeriLotTransactionDecreaseCommand { get; }
	public Command ConfirmSeriLotTransactionCommand { get; }
	public Command SeriLotTransactionCloseCommand { get; }
	#endregion
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	#endregion

	#region Properties
	public ContentPage CurrentPage { get; set; } = null!;

	#endregion


	private async Task ShowProductViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(OutputProductProcessProductListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel
			});
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

	private async Task IncreaseAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item is not null)
			{
				SelectedItem = item;

				// Stok Yeri takipli ise locationTransactionBottomSheet aç
				if (item.LocTracking == 1)
				{
					await LoadLocationTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				// Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
				else if(item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
				{
					await LoadSeriLotTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				// Stok yeri ve SeriLot takipli değilse
				else
				{
					if(item.Quantity < item.StockQuantity)
						item.Quantity++;
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

	private async Task DecreaseAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(item is not null)
			{
				if (item.Quantity > 1)
				{
					// Stok Yeri takipli ise locationTransactionBottomSheet aç
					if(item.LocTracking == 1)
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

	private async Task DeleteItemAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

			if (!result)
				return;

			if(SelectedItem == item)
			{
				SelectedItem.IsSelected = false;
				SelectedItem = null;
			}

			Items.Remove(item);
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

	private async Task LoadLocationTransactionsAsync()
	{
		try
		{
			_userDialogs.ShowLoading("Load Location Items...");
			await Task.Delay(1000);
			LocationTransactions.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedItem.ItemReferenceId,
				warehouseNumber: WarehouseModel.Number
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
				foreach (var item in result.Data)
				{
					LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
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

	private async Task LoadMoreLocationTransactionsAsync()
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

			if (item is not null)
			{
				//SelectedLocationTransaction = item;
				// SeriLot takipli ise serilotTransactionBottomSheet aç
				if (SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2) {
					await LoadSeriLotTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				else
				{
					if(item.OutputQuantity < item.Quantity)
						item.OutputQuantity++;
				}

				if (item.OutputQuantity > 0 && !item.IsSelected)
					item.IsSelected = true;

			}
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
				if(SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2)
				{
					await LoadSeriLotTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				// SeriLot takipli değilse
				else
				{
					if(item.OutputQuantity > 0)
						item.OutputQuantity--;

					if(item.OutputQuantity == 0)
						item.IsSelected = false;
				}
			}
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

	private void ConfirmLocationTransactionAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (LocationTransactions.Count > 0)
			{
				SelectedLocationTransactions.Clear();
				SelectedLocationTransactions.ToList().AddRange(LocationTransactions.Where(x => x.OutputQuantity > 0));

				foreach (var item in SelectedLocationTransactions)
				{
					SelectedItem.Details.Add(new OutputProductBasketDetailModel
					{
						LocationReferenceId = item.ReferenceId,
						LocationCode = item.LocationCode,
						LocationName = item.LocationName,
						TransactionReferenceId = item.TransactionReferenceId,
						TransactionFicheReferenceId = item.TransactionFicheReferenceId,
						InTransactionReferenceId = item.InTransactionReferenceId,
						Quantity = item.OutputQuantity,
						RemainingQuantity = item.Quantity - item.OutputQuantity,
					});
				}



				var totalOutputQuantity = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
				SelectedItem.Quantity = totalOutputQuantity;


				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
			}
			else
			{
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
			}
		}
		catch (Exception ex)
		{
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
			var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, search: string.Empty);

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

	private async Task LoadMoreSeriLotTransactionsAsync()
	{
		if (IsBusy)
			return;
		if (SeriLotTransactions.Count < (18))  // 18 = Take (20) - Remaining ItemsThreshold (2)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, skip: SeriLotTransactions.Count, take: 20);

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
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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

			if (item is not null)
			{
				SelectedSeriLotTransaction = item;
				if (item.OutputQuantity < item.Quantity)
				{
					item.OutputQuantity++;

					if (item.OutputQuantity > 0 && !item.IsSelected)
						item.IsSelected = true;
				}
			}
		}
		catch(Exception ex)
		{
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
				SelectedSeriLotTransaction = item;
				if(item.OutputQuantity == 0)
					item.IsSelected = false;
				if (item.OutputQuantity > 0)
				{
					item.OutputQuantity--;
				}
			}
		}
		catch (Exception ex)
		{
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

			if (SeriLotTransactions.Count > 0)
			{
				SelectedSeriLotTransactions.Clear();
				SelectedSeriLotTransactions.ToList().AddRange(SeriLotTransactions.Where(x => x.OutputQuantity > 0));

				// Stok yeri takipli değilse
				if (SelectedItem.LocTracking == 0) {

					foreach(var item in SelectedSeriLotTransactions)
					{
						SelectedItem.Details.Add(new OutputProductBasketDetailModel
						{
							SeriLotReferenceId = item.ReferenceId,
							SeriLotCode = item.SerilotCode,
							SeriLotName = item.SerilotName,
							TransactionFicheReferenceId = item.TransactionFicheReferenceId,
							TransactionReferenceId = item.TransactionReferenceId,
							InTransactionReferenceId = item.InTransactionReferenceId,
							Quantity = item.OutputQuantity,
							InSerilotTransactionReferenceId = item.InSerilotTransactionReferenceId,
							RemainingQuantity = item.Quantity - item.OutputQuantity
						});
					}


					var totalOutputQuantity = SeriLotTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
					SelectedItem.Quantity = totalOutputQuantity;

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
			else
			{
				CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
			}
		}
		catch (Exception ex)
		{
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (Items.Count == 0)
			{
				await _userDialogs.AlertAsync("Sepetinizde ürün bulunmamaktadır.", "Hata", "Tamam");
				return;
			}

            await Shell.Current.GoToAsync($"{nameof(OutputProductProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(OutputProductProcessType)] = OutputProductProcessType
			});
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
