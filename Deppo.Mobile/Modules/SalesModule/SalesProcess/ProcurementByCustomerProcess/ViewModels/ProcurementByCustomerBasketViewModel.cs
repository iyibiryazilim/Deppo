using System;
using System.Collections.ObjectModel;
using Android.Content;
using Android.Net.Wifi.Rtt;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;
using DevExpress.Maui.Controls;
using Kotlin.Properties;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

[QueryProperty(name: nameof(ProcurementCustomerBasketModel), queryId: nameof(ProcurementCustomerBasketModel))]
[QueryProperty(name: nameof(OrderWarehouseModel), queryId: nameof(OrderWarehouseModel))]
public partial class ProcurementByCustomerBasketViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IProcurementByCustomerBasketService _procurementByCustomerBasketService;
	private readonly IUserDialogs _userDialogs;
	private readonly ILocationService _locationService;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	private ProcurementCustomerBasketModel procurementCustomerBasketModel;

	[ObservableProperty]
	private WarehouseModel orderWarehouseModel;

	[ObservableProperty]
	ProcurementCustomerFormModel procurementCustomerFormModel = new();


	[ObservableProperty]
	ProcurementCustomerBasketProductModel selectedProcurementCustomerBasketProductModel;

	public ObservableCollection<ProcurementCustomerBasketModel> Items { get; } = new();

	public ObservableCollection<ProcurementCustomerLocationDataModel> DataItems { get; } = new();

	public ObservableCollection<LocationModel> Locations { get; } = new();

	[ObservableProperty]
	LocationModel selectedLocationModel;


	[ObservableProperty]
	private int currentPosition;

	partial void OnCurrentPositionChanged(int value)
	{
		OnPropertyChanged(nameof(IsPreviousButtonVisible));
		OnPropertyChanged(nameof(IsNextButtonVisible));
		OnPropertyChanged(nameof(IsCompleteButtonVisible));
		//OnPropertyChanged(nameof(IsPageIndicatorVisible));
	}

	[ObservableProperty]
	private int totalPosition;

	partial void OnTotalPositionChanged(int value)
	{
		OnPropertyChanged(nameof(IsNextButtonVisible));
		OnPropertyChanged(nameof(IsCompleteButtonVisible));
		//OnPropertyChanged(nameof(IsPageIndicatorVisible));
	}

	public ProcurementByCustomerBasketViewModel(
		IHttpClientService httpClientService,
		IProcurementByCustomerBasketService procurementByCustomerBasketService,
		IUserDialogs userDialogs,
		ILocationService locationService,
		IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_procurementByCustomerBasketService = procurementByCustomerBasketService;
		_userDialogs = userDialogs;
		_locationService = locationService;
		_serviceProvider = serviceProvider;

		Title = "Ürün Toplama Sepeti";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		NextPositionCommand = new Command(NextPositionAsync);
		PreviousPositionCommand = new Command(PreviousPositionAsync);
		ProcurementInfoCommand = new Command(async () => await ProcurementInfoAsync());
		GoToReasonsForRejectionListViewCommand = new Command<ProcurementCustomerBasketProductModel>(async (item) => await GoToReasonsForRejectionListViewAsync(item));
		ReverseRejectStatusCommand = new Command<ProcurementCustomerBasketProductModel>(async (item) => await ReverseRejectStatusAsync(item));

		IncreaseCommand = new Command<ProcurementCustomerBasketProductModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<ProcurementCustomerBasketProductModel>(async (item) => await DecreaseAsync(item));
		QuantityTappedCommand = new Command<ProcurementCustomerBasketProductModel>(async (item) => await QuantityTappedAsync(item));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
		ItemTappedCommand = new Command<ProcurementCustomerBasketProductModel>(async (item) => await ItemTappedAsync(item));
		LocationConfirmCommand = new Command(async () => await LocationConfirmAsync());
		LoadMoreLocationsCommand = new Command(async () => await LoadMoreLocationsAsync());
		LocationTappedCommand = new Command<LocationModel>(async (item) => await LocationTappedAsync(item));
	}

	public Page CurrentPage { get; set; }
	public bool IsPreviousButtonVisible => CurrentPosition == 0 ? false : true;
	public bool IsNextButtonVisible => CurrentPosition == TotalPosition ? false : true;
	public bool IsCompleteButtonVisible => (/*Items.Count > 0 &&*/ CurrentPosition == TotalPosition) ? true : false;
	//public bool IsPageIndicatorVisible => !IsCompleteButtonVisible;

	public Command LoadItemsCommand { get; }
	public Command NextPositionCommand { get; }
	public Command PreviousPositionCommand { get; }
	public Command ProcurementInfoCommand { get; }
	public Command GoToReasonsForRejectionListViewCommand { get; }
	public Command ReverseRejectStatusCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command QuantityTappedCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command LocationConfirmCommand { get;}
	public Command LoadMoreLocationsCommand { get; }
	public Command LocationTappedCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Yükleniyor...");
			Items.Clear();
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			List<ProcurementCustomerLocationDataModel> CacheItems = new List<ProcurementCustomerLocationDataModel>();


			var referenceIds = ProcurementCustomerBasketModel.ProcurementProductList.Select(x => x.ItemReferenceId).ToArray();
            var products = ProcurementCustomerBasketModel.ProcurementProductList;

			var result = await _procurementByCustomerBasketService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: ProcurementCustomerBasketModel.WarehouseNumber,
				itemsReferenceId: referenceIds,
				skip: 0,
				take: 99999);

			if (result.IsSuccess)
			{

				if (result.Data is not null)
				{
					foreach (var item in products)
					{
						var productLoactionData = result.Data;

						double waitingQuantity = item.WaitingQuantity;
						foreach (var prlocationData in productLoactionData)
						{
							var locationData = Mapping.Mapper.Map<ProcurementCustomerLocationDataModel>(prlocationData);
							if (locationData.ItemReferenceId == item.ItemReferenceId)
							{
								if (waitingQuantity <= 0)
									break;

								var procurementCustomerLocationDataModel = new ProcurementCustomerLocationDataModel
								{
									LocationReferenceId = locationData.LocationReferenceId,
									LocationCode = locationData.LocationCode,
									LocationName = locationData.LocationName,
									ItemReferenceId = locationData.ItemReferenceId,
									ItemCode = locationData.ItemCode,
									ItemName = locationData.ItemName,
									UnitsetReferenceId = locationData.UnitsetReferenceId,
									UnitsetCode = locationData.UnitsetCode,
									UnitsetName = locationData.UnitsetName,
									SubUnitsetReferenceId = locationData.SubUnitsetReferenceId,
									SubUnitsetCode = locationData.SubUnitsetCode,
									SubUnitsetName = locationData.SubUnitsetName,
									IsVariant = locationData.IsVariant,
									Quantity = 0,
									StockQuantity = ProcurementCustomerBasketModel.ProcurementProductList.Sum(x => x.StockQuantity),
									OrderQuantity = ProcurementCustomerBasketModel.ProcurementProductList.Sum(x => x.WaitingQuantity),
									ProcurementQuantity = waitingQuantity > locationData.ProcurementQuantity ? locationData.ProcurementQuantity : waitingQuantity,
									IsSelected = false,
									LocTracking = locationData.LocTracking,
									TrackingType = locationData.TrackingType,
									Image = locationData.Image,
								};

								CacheItems.Add(procurementCustomerLocationDataModel);
								waitingQuantity -= procurementCustomerLocationDataModel.ProcurementQuantity;
							}


						}
					}



					var groupByLocation = CacheItems
						.OrderBy(x => x.LocationCode)
						.GroupBy(x => new
						{
							LocationReferenceId = x.LocationReferenceId,
							LocationCode = x.LocationCode,
							LocationName = x.LocationName,
							WarehouseNumber = x.WarehouseNumber,
							WarehouseName = x.WarehouseName
						});

					foreach (var group in groupByLocation)
					{
						var procurementCustomerBasketModel = new ProcurementCustomerBasketModel
						{
							LocationReferenceId = group.Key.LocationReferenceId,
							LocationCode = group.Key.LocationCode,
							LocationName = group.Key.LocationName,
							WarehouseNumber = ProcurementCustomerBasketModel.WarehouseNumber,
							WarehouseName = ProcurementCustomerBasketModel.WarehouseName,
						};

						foreach (var item in group)
						{
							var product = products.FirstOrDefault(x => x.ItemReferenceId == item.ItemReferenceId);
							var procurementCustomerBasketProductModel = new ProcurementCustomerBasketProductModel
							{
								ItemReferenceId = item.ItemReferenceId,
								ItemCode = item.ItemCode,
								ItemName = item.ItemName,
								UnitsetReferenceId = item.UnitsetReferenceId,
								UnitsetCode = item.UnitsetCode,
								UnitsetName = item.UnitsetName,
								SubUnitsetReferenceId = item.SubUnitsetReferenceId,
								SubUnitsetCode = item.SubUnitsetCode,
								SubUnitsetName = item.SubUnitsetName,
								IsVariant = item.IsVariant,
								Quantity = 0,
								DestinationLocationCode = OrderWarehouseModel.LocationCode,
								//StockQuantity = ProcurementCustomerBasketModel.ProcurementProductList.Sum(x => x.StockQuantity),
								//OrderQuantity = ProcurementCustomerBasketModel.ProcurementProductList.Sum(x => x.WaitingQuantity),
								StockQuantity = ProcurementCustomerBasketModel.ProcurementProductList.FirstOrDefault(x => x.ItemReferenceId == item.ItemReferenceId).StockQuantity,
								OrderQuantity = ProcurementCustomerBasketModel.ProcurementProductList.FirstOrDefault(x => x.ItemReferenceId == item.ItemReferenceId).WaitingQuantity,
								ProcurementQuantity = item.ProcurementQuantity,
								IsSelected = false,
								LocTracking = item.LocTracking,
								TrackingType = item.TrackingType,
								Image = item.Image,
							};

							foreach (var order in product.Orders)
							{
								procurementCustomerBasketProductModel.Orders.Add(order);
							}

							procurementCustomerBasketModel.Products.Add(procurementCustomerBasketProductModel);
						}

						Items.Add(procurementCustomerBasketModel);
					}
				}

				TotalPosition = Items.Count == 0 ? 0 : Items.Count - 1;
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (System.Exception ex)
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


	private async Task IncreaseAsync(ProcurementCustomerBasketProductModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (!string.IsNullOrEmpty(item.RejectionCode))
			return;
		try
		{
			IsBusy = true;

			if (item.ProcurementQuantity > item.Quantity && item.StockQuantity > item.Quantity)
			{
				item.Quantity++;
			}
			else if (item.Quantity >= item.StockQuantity)
			{
				await _userDialogs.AlertAsync($"Stok miktarı ({item.StockQuantity}) kadar arttırabilirsiniz.", "Uyarı", "Tamam");
				return;
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

	private async Task DecreaseAsync(ProcurementCustomerBasketProductModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (!string.IsNullOrEmpty(item.RejectionCode))
			return;
		try
		{
			if (item.Quantity > 0)
			{
				item.Quantity--;
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
	private async Task ItemTappedAsync(ProcurementCustomerBasketProductModel item)
	{
		if (OrderWarehouseModel.LocationCount <= 0)
			return;

		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedProcurementCustomerBasketProductModel = item;
			await LoadLocationsAsync();
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.HalfExpanded;

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message,"Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LocationTappedAsync(LocationModel locationModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectedLocationModel = locationModel;
			if (locationModel.IsSelected)
			{
				SelectedLocationModel.IsSelected = false;
				SelectedLocationModel = null;
			}
			else
			{
				Locations.ToList().ForEach(x => x.IsSelected = false);
				SelectedLocationModel = locationModel;
				SelectedLocationModel.IsSelected = true;
			}

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

	private async Task LocationConfirmAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var selectedLocation = Locations.FirstOrDefault(x => x.IsSelected);
			if(selectedLocation is null)
			{
				await _userDialogs.AlertAsync("Lütfen bir raf seçiniz","Uyarı", "Tamam");
				return;
			}

			SelectedProcurementCustomerBasketProductModel.DestinationLocationCode = selectedLocation.Code;
			SelectedProcurementCustomerBasketProductModel.DestinationLocationReferenceId = selectedLocation.ReferenceId;
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;

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


	private async Task LoadLocationsAsync()
	{
		
		try
		{
			Locations.Clear();
			_userDialogs.Loading("Loading Locations...");
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: OrderWarehouseModel.Number,
				search: string.Empty,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
			}

			foreach (var location in result.Data)
			{
				var obj = Mapping.Mapper.Map<LocationModel>(location);

				obj.IsSelected = SelectedProcurementCustomerBasketProductModel.DestinationLocationCode == obj.Code ? true : false;
				Locations.Add(obj);
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task LoadMoreLocationsAsync()
	{
		if (IsBusy)
			return;
		if (Locations.Count < 18)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading More Locations...");
			

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: OrderWarehouseModel.Number,
				search: string.Empty,
				skip: Locations.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
			}

			foreach (var location in result.Data)
			{
				var obj = Mapping.Mapper.Map<LocationModel>(location);

				obj.IsSelected = SelectedProcurementCustomerBasketProductModel.DestinationLocationCode == obj.Code ? true : false;
				Locations.Add(obj);
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}



	private async Task QuantityTappedAsync(ProcurementCustomerBasketProductModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (!string.IsNullOrEmpty(item.RejectionCode))
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
				await _userDialogs.AlertAsync($"Girilen miktar, stok miktarını ({item.StockQuantity}) aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.ProcurementQuantity)
			{
				await _userDialogs.AlertAsync($"Girilen miktar, ürünün toplanabilir miktarını ({item.ProcurementQuantity}) aşmamalıdır.", "Hata", "Tamam");
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

	private async Task GoToReasonsForRejectionListViewAsync(ProcurementCustomerBasketProductModel item)
	{
		if (IsBusy)
			return;

		if (item is null)
			return;
		if (item.ProcurementStatusText == "Tamamlandı")
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ProcurementByCustomerReasonsForRejectionListView)}", new Dictionary<string, object>
			{
				[nameof(ProcurementCustomerBasketProductModel)] = item
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

	private async Task ReverseRejectStatusAsync(ProcurementCustomerBasketProductModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (string.IsNullOrEmpty(item.RejectionCode))
			return;
		try
		{
			item.RejectionCode = string.Empty;
			item.RejectionName = string.Empty;
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

	private async void NextPositionAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			Console.WriteLine(Items);

			bool canNext = true;

			foreach (var product in Items[CurrentPosition].Products)
			{
				if (product.ProcurementQuantity > product.Quantity && product.RejectionCode == string.Empty)
				{
					canNext = false;
					_userDialogs.ShowSnackbar(message: $"Lütfen {product.ItemCode} kodlu ürünün tamamını toplayınız ya da hata kodu ekleyiniz!");
				}
			}

			if (CurrentPosition != TotalPosition && canNext)
				CurrentPosition = CurrentPage.FindByName<CarouselView>("carouselView").Position + 1;
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

	private void PreviousPositionAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (CurrentPosition == 0)
				return;

			CurrentPosition = CurrentPage.FindByName<CarouselView>("carouselView").Position - 1;
		}
		catch (System.Exception)
		{
			_userDialogs.Alert("Bir hata oluştu.", "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	public async Task ProcurementInfoAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("procurementInfoBottomSheet").State = BottomSheetState.HalfExpanded;
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (Items.Count == 0)
			{
				await _userDialogs.AlertAsync("Herhangi bir toplanabilecek ürününüz yok.", "Hata", "Tamam");
				return;
			}

			bool hasProductsWithQuantity = Items.Any(basket => basket.Products.Any(p => p.Quantity > 0));

			if (hasProductsWithQuantity == false)
			{
				await _userDialogs.AlertAsync("Herhangi bir toplanan ürününüz yok.", "Hata", "Tamam");
				return;
			}

			bool canNext = true;

			foreach (var product in Items[CurrentPosition].Products)
			{
				if (product.ProcurementQuantity > product.Quantity && product.RejectionCode == string.Empty)
				{
					canNext = false;
					_userDialogs.ShowSnackbar(message: $"Lütfen {product.ItemCode} kodlu ürünün tamamını toplayınız ya da hata kodu ekleyiniz!");
				}
			}

			if (canNext)
			{
				var confirm = await _userDialogs.ConfirmAsync("Toplanan ürünlerle devam istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
				if (!confirm)
					return;

				var filteredBaskets = Items
										.Where(basket => basket.Products.Any(p => p.Quantity > 0)) // Products'ta Quantity > 0 olanları filtrele
										.Select(basket => new ProcurementCustomerBasketModel
										{
											CustomerReferenceId = basket.CustomerReferenceId,
											CustomerCode = basket.CustomerCode,
											CustomerName = basket.CustomerName,
											ShipAddressReferenceId = basket.ShipAddressReferenceId,
											ShipAddressCode = basket.ShipAddressCode,
											ShipAddressName = basket.ShipAddressName,
											LocationReferenceId = basket.LocationReferenceId,
											LocationCode = basket.LocationCode,
											LocationName = basket.LocationName,
											WarehouseNumber = basket.WarehouseNumber,
											WarehouseName = basket.WarehouseName,
											Products = basket.Products.Where(p => p.Quantity > 0).ToList(),
											ProcurementProductList = basket.ProcurementProductList
										})
										.ToList();


				await ConvertItems(filteredBaskets);

				await Shell.Current.GoToAsync($"{nameof(ProcurementByCustomerFormView)}", new Dictionary<string, object>
				{
					["Items"] = filteredBaskets,
					[nameof(ProcurementCustomerFormModel)] = ProcurementCustomerFormModel,
					[nameof(ProcurementCustomerBasketModel)] = ProcurementCustomerBasketModel,
					[nameof(OrderWarehouseModel)] = OrderWarehouseModel
				});
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


	private async Task ConvertItems(List<ProcurementCustomerBasketModel> items)
	{
		foreach (var item in items)
		{
			foreach (var product in item.Products)
			{
				if (ProcurementCustomerFormModel.Products.Any(x => x.ItemReferenceId == product.ItemReferenceId))
				{
					var existingProduct = ProcurementCustomerFormModel.Products.FirstOrDefault(x => x.ItemReferenceId == product.ItemReferenceId);
					existingProduct.Quantity += product.Quantity;

					existingProduct.Locations.Add(new Core.Models.LocationModels.LocationModel
					{
						ReferenceId = item.LocationReferenceId,
						Code = item.LocationCode,
						Name = item.LocationName,
						InputQuantity = product.Quantity,
					});

				}
				else
				{
					ProcurementCustomerFormModel.Products.Add(new ProcurementCustomerBasketProductModel
					{
						ItemReferenceId = product.ItemReferenceId,
						ItemCode = product.ItemCode,
						ItemName = product.ItemName,
						UnitsetReferenceId = product.UnitsetReferenceId,
						UnitsetCode = product.UnitsetCode,
						UnitsetName = product.UnitsetName,
						SubUnitsetReferenceId = product.SubUnitsetReferenceId,
						SubUnitsetCode = product.SubUnitsetCode,
						SubUnitsetName = product.SubUnitsetName,
						Quantity = product.Quantity,
						StockQuantity = product.StockQuantity,
						OrderQuantity = product.OrderQuantity,
						ProcurementQuantity = product.ProcurementQuantity,
						IsVariant = product.IsVariant,
						LocTracking = product.LocTracking,
						DestinationLocationCode = product.DestinationLocationCode,
						DestinationLocationReferenceId = product.DestinationLocationReferenceId,
						TrackingType = product.TrackingType,
						Image = product.Image,
						Locations = new List<Core.Models.LocationModels.LocationModel>
						{
							new Core.Models.LocationModels.LocationModel
							{
								ReferenceId = item.LocationReferenceId,
								Code = item.LocationCode,
								Name = item.LocationName,
								InputQuantity = product.Quantity,
							}
						},
						Orders = product.Orders

					});

				}
			}
		}


	}

	private async Task BackAsync()
	{
		try
		{
			

			var confirm = await _userDialogs.ConfirmAsync("Verileriniz silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			Items.Clear();


			await Shell.Current.GoToAsync("..");

			var previousViewModel = _serviceProvider.GetRequiredService<ProcurementByCustomerProductListViewModel>();
			await previousViewModel.LoadItemsAsync();
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