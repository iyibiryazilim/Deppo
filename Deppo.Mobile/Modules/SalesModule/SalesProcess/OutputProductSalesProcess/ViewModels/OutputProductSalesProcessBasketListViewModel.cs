using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CameraModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductSalesProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly ISeriLotTransactionService _seriLotTransactionService;
	private readonly IUserDialogs _userDialogs;
	private readonly IBarcodeSearchOutHelper _barcodeSearchOutHelper;
	private readonly ISubUnitsetService _subUnitsetService;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	OutputSalesBasketModel? selectedItem;


	[ObservableProperty]
	GroupLocationTransactionModel? selectedLocationTransaction;
	
	[ObservableProperty]
	SeriLotTransactionModel? selectedSeriLotTransaction;


	[ObservableProperty]
	public ObservableCollection<GroupLocationTransactionModel> selectedLocationTransactions = new();
	[ObservableProperty]
	public ObservableCollection<SeriLotTransactionModel> selectedSeriLotTransactions = new();

	[ObservableProperty]
	public Entry barcodeEntry;

	public ObservableCollection<OutputSalesBasketModel> Items { get; } = new();

	public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();
	public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();

	public OutputProductSalesProcessBasketListViewModel(IHttpClientService httpClientService, ILocationTransactionService locationTransactionService, ISeriLotTransactionService seriLotTransactionService, IUserDialogs userDialogs, ISubUnitsetService subUnitsetService, IBarcodeSearchOutHelper barcodeSearchOutHelper)
	{
		_httpClientService = httpClientService;
		_locationTransactionService = locationTransactionService;
		_seriLotTransactionService = seriLotTransactionService;
		_userDialogs = userDialogs;
		_subUnitsetService = subUnitsetService;
		_barcodeSearchOutHelper = barcodeSearchOutHelper;

		Title = "Sepet Listesi";
		Items.Clear();

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));

		UnitActionTappedCommand = new Command<OutputSalesBasketModel>(async (item) => await UnitActionTappedAsync(item));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (item) => await SubUnitsetTappedAsync(item));

		QuantityTappedCommand = new Command<OutputSalesBasketModel>(async (item) => await QuantityTappedAsync(item));
		IncreaseCommand = new Command<OutputSalesBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<OutputSalesBasketModel>(async (item) => await DecreaseAsync(item));
		DeleteItemCommand = new Command<OutputSalesBasketModel>(async (item) => await DeleteItemAsync(item));

		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreWarehouseLocationTransactionsAsync());
		LocationTransactionQuantityTappedCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionQuantityTappedAsync(item));
		LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
		LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
		LocationTransactionConfirmCommand = new Command(async () => await LocationTransactionConfirmAsync());
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

		LoadMoreSeriLotTransactionsCommand = new Command(async () => await LoadMoreSeriLotTransactionsAsync());
		SeriLotTransactionIncreaseCommand = new Command<SeriLotTransactionModel>(async (item) => await SeriLotTransactionIncreaseAsync(item));
		SeriLotTransactionDecreaseCommand = new Command<SeriLotTransactionModel>(async (item) => await SeriLotTransactionDecreaseAsync(item));
		SeriLotTransactionConfirmCommand = new Command(() => ConfirmSeriLotTransactionAsync());
		SeriLotTransactionCloseCommand = new Command(async () => await SeriLotTransactionCloseAsync());

		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());
		
	}

	public Page CurrentPage { get; set; } = null!;

	public Command ShowProductViewCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command UnitActionTappedCommand { get; }
	public Command SubUnitsetTappedCommand { get; }

	public Command<OutputSalesBasketModel> QuantityTappedCommand { get; }
	public Command<OutputSalesBasketModel> IncreaseCommand { get; }
	public Command<OutputSalesBasketModel> DecreaseCommand { get; }
	public Command<OutputSalesBasketModel> DeleteItemCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	public Command CameraTappedCommand { get; }

	public Command LoadMoreLocationTransactionsCommand { get; }
	public Command LocationTransactionIncreaseCommand { get; }
	public Command LocationTransactionDecreaseCommand { get; }
	public Command LocationTransactionQuantityTappedCommand { get; }
	public Command LocationTransactionConfirmCommand { get; }
	public Command LocationTransactionCloseCommand { get; }

	public Command LoadMoreSeriLotTransactionsCommand { get; }
	public Command SeriLotTransactionIncreaseCommand { get; }
	public Command SeriLotTransactionDecreaseCommand { get; }
	public Command SeriLotTransactionConfirmCommand { get; }
	public Command SeriLotTransactionCloseCommand { get; }


	private async Task ShowProductViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(OutputProductSalesProcessProductListView)}", new Dictionary<string, object> 
			{
				[nameof(WarehouseModel)] = WarehouseModel,
			});
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

	private async Task PerformSearchAsync(Entry barcodeEntry)
	{
		if (IsBusy)
			return;
		try
		{
			if (string.IsNullOrEmpty(barcodeEntry.Text))
				return;

			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _barcodeSearchOutHelper.BarcodeDetectedAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				barcode: barcodeEntry.Text,
				warehouseNumber: WarehouseModel.Number
			);

			if (result is not null)
			{
				Type resultType = result.GetType();
				if (resultType == typeof(BarcodeOutProductModel))
				{
					if (Items.Any(x => x.ItemCode == result.ProductCode))
					{
						_userDialogs.ShowToast($"{barcodeEntry.Text} barkodlu ürün sepetinizde zaten bulunmaktadır.");
						return;
					}
					else
					{
						var basketItem = new OutputSalesBasketModel
						{
							ReferenceId = Guid.NewGuid(),
							ItemReferenceId = result.ProductReferenceId,
							ItemCode = result.ProductCode,
							ItemName = result.ProductName,
							Image = result.ImageData,
							UnitsetReferenceId = result.UnitsetReferenceId,
							UnitsetCode = result.UnitsetCode,
							UnitsetName = result.UnitsetName,
							SubUnitsetReferenceId = result.SubUnitsetReferenceId,
							SubUnitsetCode = result.SubUnitsetCode,
							SubUnitsetName = result.SubUnitsetName,
							MainItemReferenceId = default,  
							MainItemCode = string.Empty,    
							MainItemName = string.Empty,
							StockQuantity = result.StockQuantity,
							IsSelected = false,   
							IsVariant = result.IsVariant,
							LocTracking = result.LocTracking,
							TrackingType = result.TrackingType,
							Quantity = result.LocTracking == 0 ? 1 : 0,
							OutputQuantity = result.LocTracking == 0 ? 1 : 0,
						};

						Items.Add(basketItem);
					}
				}
				//else if (resultType == typeof(VariantModel))
				//{
				//	if (Items.Any(x => x.ItemCode == result.Code))
				//	{
				//		_userDialogs.ShowToast($"{barcodeEntry.Text} barkodlu ürün sepetinizde zaten bulunmaktadır.");
				//		return;
				//	}
				//	else
				//	{
				//		var basketItem = new OutputSalesBasketModel
				//		{
				//			ReferenceId = Guid.NewGuid(),
				//			ItemReferenceId = result.ReferenceId,
				//			ItemCode = result.Code,
				//			ItemName = result.Name,
				//			//Image = result.ImageData,
				//			UnitsetReferenceId = result.UnitsetReferenceId,
				//			UnitsetCode = result.UnitsetCode,
				//			UnitsetName = result.UnitsetName,
				//			SubUnitsetReferenceId = result.SubUnitsetReferenceId,
				//			SubUnitsetCode = result.SubUnitsetCode,
				//			SubUnitsetName = result.SubUnitsetName,
				//			MainItemReferenceId = result.ProductReferenceId,
				//			MainItemCode = result.ProductCode,
				//			MainItemName = result.ProductName,
				//			StockQuantity = result.StockQuantity,
				//			IsSelected = false,
				//			IsVariant = true,
				//			LocTracking = result.LocTracking,
				//			TrackingType = result.TrackingType,
				//			Quantity = result.LocTracking == 0 ? 1 : 0,
				//			OutputQuantity = result.LocTracking == 0 ? 1 : 0,
				//		};

				//		Items.Add(basketItem);
				//	}
				//}
			}
			else
			{
				_userDialogs.ShowToast($"{barcodeEntry.Text} barkodunda herhangi bir ürün bulunamadı");
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			BarcodeEntry.Text = string.Empty;
			barcodeEntry.Text = string.Empty;
			barcodeEntry.Focus();
			IsBusy = false;
		}
	}

	private async Task LoadSubUnitsetsAsync(OutputSalesBasketModel item)
	{
		if (item is null)
			return;
		try
		{
			item.SubUnitsets.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _subUnitsetService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: item.ItemReferenceId
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var subUnitset in result.Data)
				{
					item.SubUnitsets.Add(Mapping.Mapper.Map<SubUnitset>(subUnitset));
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task SubUnitsetTappedAsync(SubUnitset subUnitset)
	{
		if (subUnitset is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedItem is not null)
			{
				SelectedItem.SubUnitsetReferenceId = subUnitset.ReferenceId;
				SelectedItem.SubUnitsetName = subUnitset.Name;
				SelectedItem.SubUnitsetCode = subUnitset.Code;
			}

			CurrentPage.FindByName<BottomSheet>("subUnitsetBottomSheet").State = BottomSheetState.Hidden;
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task UnitActionTappedAsync(OutputSalesBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedItem = item;
			await LoadSubUnitsetsAsync(item);
			CurrentPage.FindByName<BottomSheet>("subUnitsetBottomSheet").State = BottomSheetState.HalfExpanded;
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DeleteItemAsync(OutputSalesBasketModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item is not null)
			{
				var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
				if (!result)
				{
					return;
				}
				if(SelectedItem == item)
				{
					SelectedItem.IsSelected = false;
					SelectedItem.Details.Clear();
					SelectedItem = null;
				}

				Items.Remove(item);
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

	private async Task QuantityTappedAsync(OutputSalesBasketModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (item.LocTracking == 1)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: item.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.Quantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);
			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.StockQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, stok miktarını aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			item.Quantity = quantity;
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task IncreaseAsync(OutputSalesBasketModel outputSalesBasketModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectedItem = outputSalesBasketModel;
			// Stok Yeri Takipli olma durumu
			if (outputSalesBasketModel.LocTracking == 1)
			{
				await LoadWarehouseLocationTransactionsAsync(outputSalesBasketModel);
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;

			}
			else if (outputSalesBasketModel.TrackingType == 1 && outputSalesBasketModel.LocTracking == 0)
			{
				await LoadSeriLotTransactionsAsync();
				CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
			}
			else
			{
				if (outputSalesBasketModel.Quantity < outputSalesBasketModel.StockQuantity)
					outputSalesBasketModel.Quantity += 1;
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

	private async Task DecreaseAsync(OutputSalesBasketModel outputSalesBasketModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (outputSalesBasketModel is not null)
			{
				SelectedItem = outputSalesBasketModel;
				if (outputSalesBasketModel.Quantity > 1)
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
						outputSalesBasketModel.Quantity -= 1;
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
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
				httpClient: httpClient, 
				firmNumber: _httpClientService.FirmNumber, 
				periodNumber: _httpClientService.PeriodNumber, 
				productReferenceId: outputSalesBasketModel.IsVariant ? outputSalesBasketModel.MainItemReferenceId : outputSalesBasketModel.ItemReferenceId, 
				variantReferenceId: outputSalesBasketModel.IsVariant ? outputSalesBasketModel.ItemReferenceId : 0,
				warehouseNumber: WarehouseModel.Number, 
				skip: 0, 
				take: 20, 
				search: "");
			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
					var matchedItem = outputSalesBasketModel.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
                    if (matchedItem is not null)
						obj.OutputQuantity = matchedItem.Quantity;

                    LocationTransactions.Add(obj);
                }
			}

			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task LoadMoreWarehouseLocationTransactionsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedItem.IsVariant ? SelectedItem.MainItemReferenceId : SelectedItem.ItemReferenceId,
				variantReferenceId: SelectedItem.IsVariant ? SelectedItem.ItemReferenceId : 0,
				warehouseNumber: WarehouseModel.Number,
				skip: LocationTransactions.Count,
				take: 20,
				search: "");

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
                    var matchedItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
                    if (matchedItem is not null)
                        obj.OutputQuantity = matchedItem.Quantity;

                    LocationTransactions.Add(obj);
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

	private async Task LocationTransactionQuantityTappedAsync(GroupLocationTransactionModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: item.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.OutputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);
			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.RemainingQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, kalan miktarı aşmamalıdır.", "Hata", "Tamam");
				return;
			}
			if (quantity > SelectedItem?.StockQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, stok miktarını aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			var totalQuantity = LocationTransactions.Where(x=>x.LocationReferenceId != item.LocationReferenceId).Sum(x => x.OutputQuantity);
			if (SelectedItem?.StockQuantity >= totalQuantity + quantity)
			{
				item.OutputQuantity = quantity;
			}
			else
			{
				await _userDialogs.AlertAsync($"Girilen miktar, Ürünün stok miktarını ({SelectedItem?.StockQuantity}) aşmamalıdır.", "Hata", "Tamam");
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LocationTransactionIncreaseAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item is not null)
			{
				if (SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2)
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

	private async Task LocationTransactionDecreaseAsync(GroupLocationTransactionModel item)
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

					if (item.OutputQuantity == 0)
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

	private async Task LocationTransactionConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (LocationTransactions.Count > 0)
			{
				foreach (var item in LocationTransactions.Where(x => x.OutputQuantity <= 0))
				{
					SelectedItem.Details.Remove(SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId));
				}

				SelectedLocationTransactions.Clear();

                foreach (var x in LocationTransactions.Where(x => x.OutputQuantity > 0))
                {
					SelectedLocationTransactions.Add(x);
                }

                foreach (var item in SelectedLocationTransactions)
                {
					var selectedLocationTransactionItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId);
					if(selectedLocationTransactionItem is not null)
					{
						selectedLocationTransactionItem.Quantity = item.OutputQuantity;
					}
					else
					{
                        SelectedItem.Details.Add(new OutputSalesBasketDetailModel
                        {
                            //ReferenceId = item.ReferenceId,
                            LocationReferenceId = item.LocationReferenceId,
                            LocationCode = item.LocationCode,
                            LocationName = item.LocationName,
                            Quantity = item.OutputQuantity,
                            RemainingQuantity = item.RemainingQuantity,
                        });
                    }

					
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

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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

	private async Task SeriLotTransactionIncreaseAsync(SeriLotTransactionModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item is not null)
			{
				if (item.OutputQuantity < item.Quantity)
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

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task SeriLotTransactionDecreaseAsync(SeriLotTransactionModel item)
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

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
				if (SelectedItem.LocTracking == 0)
				{
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
			} else
			{
				CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
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


			bool isQuantityValid = Items.All(x => x.Quantity > 0);
			if (!isQuantityValid)
			{
				await _userDialogs.AlertAsync("Sepetinizde miktarı 0 olan ürünler bulunmaktadır.", "Uyarı", "Tamam");
				return;
			}

			await Shell.Current.GoToAsync($"{nameof(OutputProductSalesProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(Items)] = Items,
			});

            CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;

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
				foreach (var item in Items)
				{
					item.Details.Clear();
				}
				Items.Clear();
				await Shell.Current.GoToAsync("..");
			}
			else
				await Shell.Current.GoToAsync("..");

            CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;

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

	private async Task CameraTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CameraScanModel cameraScanModel = new CameraScanModel
			{
				ComingPage = "OutputProductSalesProcessBasketListViewModel",
				WarehouseNumber = WarehouseModel.Number,
				ShipInfoReferenceId = 0,
				CurrentReferenceId = 0
			};

			await Shell.Current.GoToAsync($"{nameof(CameraReaderView)}", new Dictionary<string, object>
			{
				[nameof(CameraScanModel)] = cameraScanModel
			});
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
}
