﻿using AutoMapper.Execution;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using DevExpress.Data.Async.Helpers;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class OutputProductSalesOrderProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly ISeriLotTransactionService _seriLotTransactionService;
	private readonly IBarcodeSearchHelper _barcodeSearchHelper;
	private readonly ISubUnitsetService _subUnitsetService;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	[ObservableProperty]
	public Entry barcodeEntry;

	[ObservableProperty]
	public SearchBar locationTransactionSearchText;

	public ObservableCollection<OutputSalesBasketModel> Items { get; } = new();

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

	public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();

	//Arama yapıldığında kullanılan liste
	public ObservableCollection<GroupLocationTransactionModel> SelectedLocationTransactionItems { get; } = new();
	public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();

	public OutputProductSalesOrderProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationTransactionService locationTransactionService, ISeriLotTransactionService seriLotTransactionService, IBarcodeSearchHelper barcodeSearchHelper, ISubUnitsetService subUnitsetService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationTransactionService = locationTransactionService;
		_seriLotTransactionService = seriLotTransactionService;
		_barcodeSearchHelper = barcodeSearchHelper;
		_subUnitsetService = subUnitsetService;

		Title = "Satış Sepeti";

		PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));
		UnitActionTappedCommand = new Command<OutputSalesBasketModel>(async (item) => await UnitActionTappedAsync(item));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (subUnitset) => await SubUnitsetTappedAsync(subUnitset));
		QuantityTappedCommand = new Command<OutputSalesBasketModel>(async (item) => await QuantityTappedAsync(item));
		IncreaseCommand = new Command<OutputSalesBasketModel>(async (outputSalesBasketModel) => await IncreaseAsync(outputSalesBasketModel));
		DecreaseCommand = new Command<OutputSalesBasketModel>(async (outputSalesBasketModel) => await DecreaseAsync(outputSalesBasketModel));
		DeleteItemCommand = new Command<OutputSalesBasketModel>(async (outputSalesBasketModel) => await DeleteItemAsync(outputSalesBasketModel));
		BackCommand = new Command(async () => await BackAsync());
		PlusTappedCommand = new Command(async () => await PlusTappedAsync());
		ProductOptionTappedCommand = new Command(async () => await ProductOptionTappedAsync());
		OrderOptionTappedCommand = new Command(async () => await OrderOptionTappedAsync());
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());

		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreWarehouseLocationTransactionsAsync());
		LocationTransactionQuantityTappedCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionQuantityTappedAsync(item));
		LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
		LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
		LocationTransactionConfirmCommand = new Command(ConfirmLocationTransactionAsync);
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());
		LocationTransactionPerformSearchCommand = new Command(async () => await LocationTransactionPerformSearchAsync());
		LocationTransactionPerformEmptySearchCommand = new Command(async () => await LocationTransactionPerformEmptySearchAsync());

		LoadMoreSeriLotTransactionsCommand = new Command(async () => await LoadMoreSeriLotTransactionsAsync());
		SeriLotTransactionIncreaseCommand = new Command<SeriLotTransactionModel>(async (item) => SeriLotTransactionIncreaseAsync(item));
		SeriLotTransactionDecreaseCommand = new Command<SeriLotTransactionModel>(async (item) => SeriLotTransactionDecreaseAsync(item));
		SeriLotTransactionConfirmCommand = new Command(ConfirmSeriLotTransactionAsync);
		SeriLotTransactionCloseCommand = new Command(async () => await SeriLotTransactionCloseAsync());
	}
	public ContentPage CurrentPage { get; set; } = null!;

	#region Commands
	public Command PerformSearchCommand { get; }
	public Command UnitActionTappedCommand { get; }
	public Command SubUnitsetTappedCommand { get; }

	public Command QuantityTappedCommand { get; }
	public Command<OutputSalesBasketModel> IncreaseCommand { get; }
	public Command<OutputSalesBasketModel> DecreaseCommand { get; }
	public Command<OutputSalesBasketModel> DeleteItemCommand { get; }
	public Command PlusTappedCommand { get; }
	public Command ProductOptionTappedCommand { get; }
	public Command OrderOptionTappedCommand { get; }
	public Command CameraTappedCommand { get; }


	#region LocationTransaction Command
	public Command LoadMoreLocationTransactionsCommand { get; }
	public Command LocationTransactionQuantityTappedCommand { get; }
	public Command LocationTransactionIncreaseCommand { get; }
	public Command LocationTransactionDecreaseCommand { get; }
	public Command LocationTransactionConfirmCommand { get; }
	public Command LocationTransactionCloseCommand { get; }
    public Command LocationTransactionPerformSearchCommand { get; }
    public Command LocationTransactionPerformEmptySearchCommand { get; }
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

			await _barcodeSearchHelper.BarcodeDetectedAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				barcode: barcodeEntry.Text,
				comingPage: "OutputProductSalesOrderProcessBasketListViewModel"
			);

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

	private async Task QuantityTappedAsync(OutputSalesBasketModel item)
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
				initialValue: item.OutputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);
			if (quantity <= 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.Quantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, ürünün stok miktarını aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			item.OutputQuantity = quantity;
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
				await LoadWarehouseLocationTransactionsAsync();
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;

			}
			else if (outputSalesBasketModel.TrackingType == 1 ||outputSalesBasketModel.TrackingType == 2)
			{
				CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
			}
			else
			{
				if (outputSalesBasketModel.OutputQuantity < outputSalesBasketModel.Quantity)
					outputSalesBasketModel.OutputQuantity++;
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
            SelectedItem = outputSalesBasketModel;
            if (outputSalesBasketModel is not null)
			{
				if (outputSalesBasketModel.OutputQuantity > 1)
				{
					// Stok Yeri Takipli olma durumu
					if (outputSalesBasketModel.LocTracking == 1)
					{
						await LoadWarehouseLocationTransactionsAsync();
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
						outputSalesBasketModel.OutputQuantity--;
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
	private async Task DeleteItemAsync(OutputSalesBasketModel outputSalesBasketModel)
	{
		if (outputSalesBasketModel is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync($"{outputSalesBasketModel.ItemCode}\n{outputSalesBasketModel.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			outputSalesBasketModel.Details.Clear();
			Items.Remove(outputSalesBasketModel);

		}
		catch (Exception ex) 
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadWarehouseLocationTransactionsAsync()
	{
		try
		{
			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);

			LocationTransactions.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, 0, 20, LocationTransactionSearchText.Text);
			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
                    var matchingItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
                    if (matchingItem != null)
                        obj.OutputQuantity = matchingItem.Quantity;

                    LocationTransactions.Add(Mapping.Mapper.Map<GroupLocationTransactionModel>(item));
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
		if (LocationTransactions.Count < (18))  // 18 = Take (20) - Remaining ItemsThreshold (2)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedItem.ItemReferenceId,
				warehouseNumber: WarehouseModel.Number,
				skip: LocationTransactions.Count,
				take: 20,
				search: LocationTransactionSearchText.Text);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
					var matchingItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
                    if (matchingItem != null)
						obj.OutputQuantity = matchingItem.Quantity;

                    LocationTransactions.Add(Mapping.Mapper.Map<GroupLocationTransactionModel>(item));
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
				initialValue: item.OutputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);
			if (quantity <= 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.RemainingQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, kalan miktarı aşmamalıdır.", "Hata", "Tamam");
				return;
			}
			if (quantity > SelectedItem?.Quantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, ürünün miktarını aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			var totalQuantity = LocationTransactions.Sum(x => x.OutputQuantity);
			if (SelectedItem?.Quantity >= totalQuantity + quantity)
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
				SelectedLocationTransaction = item;
				if (SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2)
				{
					await LoadSeriLotTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				else
				{
					var totalQuantity = LocationTransactions.Sum(x => x.OutputQuantity);
					if (SelectedItem.Quantity > totalQuantity)
					{
						if (item.OutputQuantity < item.RemainingQuantity && SelectedItem.Quantity > item.OutputQuantity)
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

			if (item.OutputQuantity > 0)
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
                foreach (var item in LocationTransactions.Where( x => x.OutputQuantity > 0))
                {
					SelectedLocationTransactions.Add(item);
                }

                foreach (var item in SelectedLocationTransactions)
				{
					var selectedLocationTransactionItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId);
					if (selectedLocationTransactionItem is not null)
					{
						selectedLocationTransactionItem.Quantity = item.OutputQuantity;
						selectedLocationTransactionItem.RemainingQuantity = item.OutputQuantity;
					}

					SelectedItem.Details.Add(new OutputSalesBasketDetailModel
					{
						//ReferenceId = item.ReferenceId,
						LocationReferenceId = item.LocationReferenceId,
						LocationCode = item.LocationCode,
						LocationName = item.LocationName,
						Quantity = item.OutputQuantity,
						//TransactionReferenceId = item.TransactionReferenceId,
						//TransactionFicheReferenceId = item.TransactionFicheReferenceId,
						//InTransactionReferenceId = item.InTransactionReferenceId,
						RemainingQuantity = item.OutputQuantity,
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

    private async Task LocationTransactionPerformSearchAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(LocationTransactionSearchText.Text))
            {
                await LoadWarehouseLocationTransactionsAsync();
                LocationTransactionSearchText.Unfocus();
                return;
            }
            IsBusy = true;

            LocationTransactions.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.ItemReferenceId,
                warehouseNumber: WarehouseModel.Number,
                skip: 0,
                take: 20,
                search: LocationTransactionSearchText.Text
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
                }

                foreach (var locationTransaction in LocationTransactions)
                {
                    var matchingItem = SelectedItem.Details.FirstOrDefault(item => item.LocationReferenceId == locationTransaction.LocationReferenceId);
                    if (matchingItem != null)
                    {
                        locationTransaction.OutputQuantity = matchingItem.Quantity;
                    }
                }


            }
        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LocationTransactionPerformEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(LocationTransactionSearchText.Text))
        {
            await LocationTransactionPerformSearchAsync();
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

			if (SeriLotTransactions.Count > 0)
			{
				SelectedSeriLotTransactions.Clear();
				SelectedSeriLotTransactions.ToList().AddRange(SeriLotTransactions.Where(x => x.OutputQuantity > 0));
				// Stok yeri takipli değilse
				if (SelectedItem.LocTracking == 0)
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

			bool isQuantityValid = Items.All(x => x.OutputQuantity > 0);
			if (!isQuantityValid)
			{
				await _userDialogs.AlertAsync("Sepetinizde miktarı 0 olan ürünler bulunmaktadır.", "Uyarı", "Tamam");
				return;
			}

			await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(Items)] = Items,
				[nameof(SalesCustomer)] = SalesCustomer
			});
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

	private async Task PlusTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.HalfExpanded;

		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ProductOptionTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.Hidden;

			await Task.Delay(300);
			await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessProductListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(SalesCustomer)] = SalesCustomer
			});

			
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task OrderOptionTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.Hidden;
			await Task.Delay(300);

			await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessOrderListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(SalesCustomer)] = SalesCustomer
			});
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

	private async Task CameraTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(CameraReaderView)}", new Dictionary<string, object>
			{
				["ComingPage"] = "OutputProductSalesOrderBasket"
			});
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
}
