using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MessageHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    [QueryProperty(name: nameof(InputOutsourceTransferBasketModel), queryId: nameof(InputOutsourceTransferBasketModel))]
    public partial class InputOutsourceTransferBasketLocationListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ILocationService _locationService;
        private readonly IUserDialogs _userDialogs;
        private readonly IServiceProvider _serviceProvider;

		[ObservableProperty]
		private WarehouseModel warehouseModel = null!;

		[ObservableProperty]
		private InputOutsourceTransferBasketModel inputOutsourceTransferBasketModel = null!;

		public ObservableCollection<LocationModel> Items { get; } = new();
		public ObservableCollection<LocationModel> SelectedItems { get; } = new();
        public ObservableCollection<LocationModel> SelectedSearchItems { get; } = new();

		[ObservableProperty]
		private LocationModel selectedItem;

        [ObservableProperty]
        public SearchBar searchText;

		public InputOutsourceTransferBasketLocationListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationService locationService, IServiceProvider serviceProvider)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _locationService = locationService;
            _serviceProvider = serviceProvider;

            LoadSelectedItemsCommand = new Command(async () => await LoadSelectedItemsAsync());
            ShowLocationsCommand = new Command(async () => await ShowLocationsAsync());
            PerformSearchCommand = new Command<Entry>(async (searchBar) => await PerformSearchAsync(searchBar));
			LocationsPerformSearchCommand = new Command(async () => await LocationsPerformSearchAsync(SearchText));
			LocationsPerformEmptySearchCommand = new Command(async () => await LocationsPerformEmptySearchAsync());
			QuantityTappedCommand = new Command<LocationModel>(async (locationModel) => await QuantityTappedAsync(locationModel));
            IncreaseCommand = new Command<LocationModel>(async (locationModel) => await IncreaseAsync(locationModel));
            DecreaseCommand = new Command<LocationModel>(async (locationModel) => await DecreaseAsync(locationModel));
            ConfirmCommand = new Command(async () => await ConfirmAsync());
            CancelCommand = new Command(async () => await CancelAsync());

            CloseLocationsCommand = new Command(async () => await CloseLocationsAsync());
            ItemTappedCommand = new Command<LocationModel>(async (locationModel) => await ItemTappedAsync(locationModel));
            ConfirmLocationsCommand = new Command(async () => await ConfirmLocationsAsync());
            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        }

        public Page CurrentPage { get; set; }

        public Command LoadSelectedItemsCommand { get; }
        public Command QuantityTappedCommand { get; }
        public Command<LocationModel> IncreaseCommand { get; }
        public Command<LocationModel> DecreaseCommand { get; }
        public Command<Entry> PerformSearchCommand { get; }
        public Command ConfirmCommand { get; }
        public Command CancelCommand { get; }

        #region Location BottomSheet Commands

        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ShowLocationsCommand { get; }
        public Command CloseLocationsCommand { get; }
        public Command<LocationModel> ItemTappedCommand { get; }
        public Command LocationsPerformEmptySearchCommand { get; }
        public Command LocationsPerformSearchCommand { get; }
		public Command ConfirmLocationsCommand { get; }

        #endregion Location BottomSheet Commands

        public async Task LoadSelectedItemsAsync()
        {
            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading...");
                await Task.Delay(500);

                SelectedItems.Clear();
                if (InputOutsourceTransferBasketModel.Details.Count > 0)
                {
                    foreach (var item in InputOutsourceTransferBasketModel.Details)
                        SelectedItems.Add(new LocationModel
                        {
                            Code = item.LocationCode,
                            Name = item.LocationName,
                            StockQuantity = default,
                            InputQuantity = item.Quantity
                        });
                }

                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShowLocationsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                await LoadItemsAsync();
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.HalfExpanded;
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                Items.Clear();

                _userDialogs.ShowLoading("Loading...");
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();

				var result = await _locationService.GetObjects(
					  httpClient: httpClient,
					  firmNumber: _httpClientService.FirmNumber,
					  periodNumber: _httpClientService.PeriodNumber,
					  warehouseNumber: WarehouseModel.Number,
					  productReferenceId: InputOutsourceTransferBasketModel.ItemReferenceId,
					  search: SearchText.Text,
					  skip: 0,
					  take: 20);

				if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            var obj = Mapping.Mapper.Map<LocationModel>(item);
                            var matchedItem = SelectedSearchItems.FirstOrDefault(x => x.Code == obj.Code);
                            obj.IsSelected = matchedItem != null ? matchedItem.IsSelected : false;
							Items.Add(obj);
                        }
                    }
                }

                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadMoreItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading...");
                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _locationService.GetObjects(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    warehouseNumber: WarehouseModel.Number,
                    productReferenceId: InputOutsourceTransferBasketModel.ItemReferenceId,
                    skip: Items.Count,
                    take: 20);

                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
							var obj = Mapping.Mapper.Map<LocationModel>(item);
							var matchedItem = SelectedSearchItems.FirstOrDefault(x => x.Code == obj.Code);
							obj.IsSelected = matchedItem != null ? matchedItem.IsSelected : false;
							Items.Add(obj);
						}  
                    }
                }

                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LocationsPerformSearchAsync(SearchBar searchText)
        {
            try
            {
                if(string.IsNullOrEmpty(SearchText.Text))
                {
                    await LoadItemsAsync();
                    SearchText.Unfocus();
                    return;
                }

                IsBusy = true;

				Items.Clear();
				_userDialogs.Loading("Searching...");

				var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _locationService.GetObjects(
					httpClient: httpClient,
					firmNumber: _httpClientService.FirmNumber,
					periodNumber: _httpClientService.PeriodNumber,
					warehouseNumber: WarehouseModel.Number,
					productReferenceId: InputOutsourceTransferBasketModel.ItemReferenceId,
					search: SearchText.Text,
					skip: 0,
					take: 20);  

                if(result.IsSuccess)
                {
                    if(result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
							var obj = Mapping.Mapper.Map<LocationModel>(item);
							var matchedItem = SelectedSearchItems.FirstOrDefault(x => x.Code == obj.Code);
							obj.IsSelected = matchedItem != null ? matchedItem.IsSelected : false;
							Items.Add(obj);
                        }
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
        private async Task LocationsPerformEmptySearchAsync()
        {
          if(string.IsNullOrWhiteSpace(SearchText.Text))
            {
                await LoadItemsAsync();
			}
        }
        private async Task CloseLocationsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading...");
                await Task.Delay(500);
                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ItemTappedAsync(LocationModel locationModel)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (locationModel.IsSelected)
                {
					SelectedSearchItems.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = false;
					SelectedSearchItems.Remove(Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId));
					Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = false;
                }
                else
                {
					Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = true;
					SelectedSearchItems.Add(Items.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId));
					SelectedSearchItems.FirstOrDefault(x => x.ReferenceId == locationModel.ReferenceId).IsSelected = true;
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

        private async Task ConfirmLocationsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading...");
                await Task.Delay(500);
                foreach (var item in Items.Where(x => x.IsSelected))
                {
                    if (!SelectedItems.Any(x => x.Code == item.Code))
                        SelectedItems.Add(item);
                }

                SelectedSearchItems.Clear();

                CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

		private async Task QuantityTappedAsync(LocationModel locationModel)
		{
			if (locationModel is null)
				return;
			if (IsBusy)
				return;
			try
			{
				IsBusy = true;

				var result = await CurrentPage.DisplayPromptAsync(
					title: locationModel.Code,
					message: "Miktarı giriniz",
					cancel: "Vazgeç",
					accept: "Tamam",
					placeholder: locationModel.InputQuantity.ToString(),
					keyboard: Keyboard.Numeric);

				if (string.IsNullOrEmpty(result))
					return;

				var quantity = Convert.ToDouble(result);

				if (quantity < 0)
				{
					await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
					return;
				}

				locationModel.InputQuantity = quantity;
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
		private async Task IncreaseAsync(LocationModel locationModel)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                SelectedItem = locationModel;

                if (InputOutsourceTransferBasketModel.TrackingType != 0)
                {
                    await Shell.Current.GoToAsync($"{nameof(InputProductProcessBasketSeriLotListView)}", new Dictionary<string, object>
                    {
                        [nameof(WarehouseModel)] = WarehouseModel,
                        [nameof(InputOutsourceTransferBasketModel)] = InputOutsourceTransferBasketModel
                    });
                }
                else
                {
                    locationModel.InputQuantity++;
                }
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DecreaseAsync(LocationModel locationModel)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (locationModel.InputQuantity > 0)
                {
                    SelectedItem = locationModel;

                    if (InputOutsourceTransferBasketModel.TrackingType != 0)
                    {
                        await Shell.Current.GoToAsync($"{nameof(InputProductProcessBasketSeriLotListView)}", new Dictionary<string, object>
                        {
                            [nameof(WarehouseModel)] = WarehouseModel,
                            [nameof(InputOutsourceTransferBasketModel)] = InputOutsourceTransferBasketModel
                        });
                    }
                    else
                    {
                        locationModel.InputQuantity--;
                    }
                }
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
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
                var result = await _locationService.GetObjects(
                            httpClient: httpClient,
                            firmNumber: _httpClientService.FirmNumber,
                            periodNumber: _httpClientService.PeriodNumber,
                            warehouseNumber: WarehouseModel.Number,
                            productReferenceId: InputOutsourceTransferBasketModel.IsVariant ? InputOutsourceTransferBasketModel.MainItemReferenceId :  InputOutsourceTransferBasketModel.ItemReferenceId,
                            variantReferenceId: InputOutsourceTransferBasketModel.IsVariant ? InputOutsourceTransferBasketModel.ItemReferenceId : 0,
							search: barcodeEntry.Text,
                            skip: 0,
                            take: 1);

				if (result.IsSuccess)
				{
					if (!(result.Data.Count() > 0))
					{
						if (_userDialogs.IsHudShowing)
							_userDialogs.HideHud();

						_userDialogs.ShowToast(message: $"{barcodeEntry.Text} kodlu raf bulunamadı.");
						return;
					}

					foreach (var item in result.Data)
					{
						var obj = Mapping.Mapper.Map<LocationModel>(item);
						if (SelectedItems.Where(x => x.Code == obj.Code).Any())
						{
							SelectedItems.Where(x => x.Code == obj.Code).FirstOrDefault().InputQuantity += 1;
						}
						else
						{
							SelectedItems.Add(obj);
						}
					}
				}

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();
			}
            catch (Exception ex)
            {
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				_userDialogs.Alert(ex.Message);
			}
            finally
            {
				barcodeEntry.Text = string.Empty;
				barcodeEntry.Focus();
				IsBusy = false;
            }
        }

        /// <summary>
        /// Confirm the selected locations and go back to the previous page.
        /// </summary>
        /// <returns></returns>
        private async Task ConfirmAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading...");

                var previousViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferOutsourceBasketListViewModel>();

                if (previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == InputOutsourceTransferBasketModel.ItemReferenceId) is not null)
                {
                    foreach (var item in SelectedItems.Where(x => x.InputQuantity > 0)) //Locations
                    {
                        var location = previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == InputOutsourceTransferBasketModel.ItemReferenceId).Details.FirstOrDefault(x => x.LocationCode == item.Code);
                        if (location is not null)
                        {
                            location.Quantity = item.InputQuantity;
                        }
                        else
                        {
                            previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == InputOutsourceTransferBasketModel.ItemReferenceId).Details.Add(new InputOutsourceTransferBasketDetailModel
                            {
                                LocationReferenceId = item.ReferenceId,
                                LocationCode = item.Code,
                                LocationName = item.Name,
                                Quantity = item.InputQuantity
                            });
                        }

                        var totalInputQuantity = SelectedItems.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);
                        previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == InputOutsourceTransferBasketModel.ItemReferenceId).Quantity = totalInputQuantity;
                    }
                }

                await Shell.Current.GoToAsync("..");
                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading...");
                await Task.Delay(500);
                await Shell.Current.GoToAsync("..");
                _userDialogs.HideHud();
            }
            catch (Exception ex)
            {
                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}