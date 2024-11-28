using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using DevExpress.Maui.Controls;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;
using System.Collections.ObjectModel;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.CameraModels;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels
{
    [QueryProperty(nameof(WarehouseModel), nameof(WarehouseModel))]
    public partial class DemandProcessBasketListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBarcodeSearchDemandHelper _barcodeSearchDemandHelper;
		private readonly ISubUnitsetService _subUnitsetService;

        [ObservableProperty]
        WarehouseModel warehouseModel = null!;

        [ObservableProperty]
        DemandProcessBasketModel? selectedItem;

		[ObservableProperty]
		public Entry barcodeEntry;

		#region Collections
		public ObservableCollection<DemandProcessBasketModel> Items { get; } = new();

		#endregion

		public DemandProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IServiceProvider serviceProvider, ISubUnitsetService subUnitsetService, IBarcodeSearchDemandHelper barcodeSearchDemandHelper)
		{
			_httpClientService = httpClientService;
			_userDialogs = userDialogs;
			_serviceProvider = serviceProvider;
		
			_subUnitsetService = subUnitsetService;
			_barcodeSearchDemandHelper = barcodeSearchDemandHelper;

			Title = "Sepet Listesi";

			ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
			PerformSearchCommand = new Command<Entry>(async (barcodeEntry) => await PerformSearchAsync(barcodeEntry));

			UnitActionTappedCommand = new Command<DemandProcessBasketModel>(async (item) => await UnitActionTappedAsync(item));
			SubUnitsetTappedCommand = new Command<SubUnitset>(async (subUnitset) => await SubUnitsetTappedAsync(subUnitset));

			QuantityTappedCommand = new Command<DemandProcessBasketModel>(async (item) => await QuantityTappedAsync(item));
			IncreaseCommand = new Command<DemandProcessBasketModel>(async (item) => await IncreaseAsync(item));
			DecreaseCommand = new Command<DemandProcessBasketModel>(async (item) => await DecreaseAsync(item));
			DeleteItemCommand = new Command<DemandProcessBasketModel>(async (item) => await DeleteItemAsync(item));
			NextViewCommand = new Command(async () => await NextViewAsync());
			BackCommand = new Command(async () => await BackAsync());
			CameraTappedCommand = new Command(async () => await CameraTappedAsync());
			SelectProductsCommand = new Command(async () => await SelectProductsAsync());
			SelectVariantsCommand = new Command(async () => await SelectVariantsAsync());
		}

		public ContentPage CurrentPage { get; set; } = null!;

        #region Commands
        public Command PerformSearchCommand { get; }
        public Command ShowProductViewCommand { get; }
        public Command UnitActionTappedCommand { get; }
        public Command SubUnitsetTappedCommand { get; }

        public Command QuantityTappedCommand { get; }
        public Command IncreaseCommand { get; }
        public Command DecreaseCommand { get; }
        public Command DeleteItemCommand { get; }
        public Command NextViewCommand { get; }
        public Command BackCommand { get; }
        public Command CameraTappedCommand { get; }
        public Command SelectProductsCommand { get; }
        public Command SelectVariantsCommand { get; }
		#endregion

		private async Task UnitActionTappedAsync(DemandProcessBasketModel item)
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

		private async Task LoadSubUnitsetsAsync(DemandProcessBasketModel item)
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

				var result = await _barcodeSearchDemandHelper.BarcodeDetectedAsync(
					httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    barcode: barcodeEntry.Text,
                    warehouseNumber: WarehouseModel.Number
				);

				if (result is not null)
				{
					Type resultType = result.GetType();
					if (resultType == typeof(BarcodeDemandProductModel))
					{
						if (Items.Any(x => x.ItemCode == result.ProductCode))
						{
							_userDialogs.ShowToast($"{barcodeEntry.Text} barkodlu ürün sepetinizde zaten bulunmaktadır.");
							return;
						}
						else
						{
							var basketItem = new DemandProcessBasketModel
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
								IsSelected = false,
								MainItemCode = string.Empty,
								MainItemName = string.Empty,
								MainItemReferenceId = default,
								StockQuantity = result.StockQuantity,
								SafeLevel = result.SafeLevel,
								Quantity = result.SafeLevel - result.StockQuantity, 
								LocTracking = result.LocTracking,
								TrackingType = result.TrackingType,
								IsVariant = result.IsVariant
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
					//		var basketItem = new DemandProcessBasketModel
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
					//			IsSelected = false,
					//			MainItemCode = result.ProductCode,
					//			MainItemName = result.ProductName,
					//			MainItemReferenceId = result.ProductReferenceId,
					//			StockQuantity = result.StockQuantity,
					//			//SafeLevel = result.SafeLevel,
					//			Quantity = -result.StockQuantity, // result.SafeLevel - result.StockQuantity
					//			LocTracking = result.LocTracking,
					//			TrackingType = result.TrackingType,
					//			IsVariant = true,
					//			VariantIcon = result.VariantIcon,
					//			LocTrackingIcon = result.LocTrackingIcon,
					//			TrackingTypeIcon = result.TrackingTypeIcon,
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

		private async Task ShowProductViewAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                CurrentPage.FindByName<BottomSheet>("productTypeBottomSheet").State = BottomSheetState.HalfExpanded;

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

        private async Task SelectProductsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                CurrentPage.FindByName<BottomSheet>("productTypeBottomSheet").State = BottomSheetState.Hidden;
                await Task.Delay(300);
                await Shell.Current.GoToAsync($"{nameof(DemandProcessProductListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel
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

        private async Task SelectVariantsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                CurrentPage.FindByName<BottomSheet>("productTypeBottomSheet").State = BottomSheetState.Hidden;
                await Task.Delay(300);
                await Shell.Current.GoToAsync($"{nameof(DemandProcessVariantListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel
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

		private async Task QuantityTappedAsync(DemandProcessBasketModel demandProcessBasketModel)
		{
			if (IsBusy)
				return;
			if (demandProcessBasketModel is null)
				return;
			try
			{
				IsBusy = true;

				var result = await CurrentPage.DisplayPromptAsync(
					title: demandProcessBasketModel.ItemCode,
					message: "Miktarı giriniz",
					cancel: "Vazgeç",
					accept: "Tamam",
					placeholder: demandProcessBasketModel.Quantity.ToString(),
					keyboard: Keyboard.Numeric);

				if (string.IsNullOrEmpty(result))
					return;

				var quantity = Convert.ToDouble(result);

				if (quantity < 0)
				{
					await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
					return;
				}

				demandProcessBasketModel.Quantity = quantity;
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
		private async Task DeleteItemAsync(DemandProcessBasketModel item)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

                if (!result)
                    return;

                if (SelectedItem == item)
                {
                    SelectedItem.IsSelected = false;
                    SelectedItem = null;
                }

                Items.Remove(item);
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

        private async Task IncreaseAsync(DemandProcessBasketModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (item is not null)
                {
                    SelectedItem = item;

                    item.Quantity++;

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

        private async Task DecreaseAsync(DemandProcessBasketModel item)
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                if (item is not null)
                {
                    SelectedItem = item;

                    if (item.Quantity > 0)
                        item.Quantity--;


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

				CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;

				await Shell.Current.GoToAsync($"{nameof(DemandProcessFormView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(Items)] = Items
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

                    Items.Clear();

                    var productListViewModel = _serviceProvider.GetRequiredService<DemandProcessProductListViewModel>();
                    var variantListViewModel = _serviceProvider.GetRequiredService<DemandProcessVariantListViewModel>();

                    foreach (var item in productListViewModel.Items)
                        item.IsSelected = false;

                    foreach (var item in variantListViewModel.Items)
                        item.IsSelected = false;

                    variantListViewModel.Items.Clear();
                    variantListViewModel.SelectedVariants.Clear();

                    productListViewModel.Items.Clear();
                    productListViewModel.SelectedProducts.Clear();

                    CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;
					await Shell.Current.GoToAsync("..");
                }
                else
                {
					CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;
					await Shell.Current.GoToAsync("..");
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

        private async Task CameraTappedAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                CameraScanModel cameraScanModel = new CameraScanModel
				{
					ComingPage = "DemandProcessBasketListViewModel",
                    CurrentReferenceId = 0,
					ShipInfoReferenceId = 0,
                    WarehouseNumber = WarehouseModel.Number
				};

				await Shell.Current.GoToAsync($"{nameof(CameraReaderView)}", new Dictionary<string, object>
                {
                    [nameof(CameraScanModel)] = cameraScanModel
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
}
