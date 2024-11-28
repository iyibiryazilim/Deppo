using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;
using System.Collections.ObjectModel;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.Views;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels
{
    public partial class DemandProcessWarehouseListViewModel : BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IWarehouseService _warehouseService;
        private readonly IUserDialogs _userDialogs;
        public DemandProcessWarehouseListViewModel(IHttpClientService httpClientService, IWarehouseService warehouseService, IUserDialogs userDialogs)
        {
            _httpClientService = httpClientService;
            _warehouseService = warehouseService;
            _userDialogs = userDialogs;

            Title = "Ambar seçimi";

            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
            LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
            ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
            NextViewCommand = new Command(async () => await NextViewAsync());
        }

        #region Commands
        public Command LoadItemsCommand { get; }
        public Command LoadMoreItemsCommand { get; }
        public Command ItemTappedCommand { get; }
        public Command NextViewCommand { get; }
        #endregion

        #region Collections
        public ObservableCollection<WarehouseModel> Items { get; } = new();
        #endregion

        #region Properties
        [ObservableProperty]
        OutputProductProcessType outputProductProcessType;

        [ObservableProperty]
        WarehouseModel selectedWarehouseModel = null!;
        #endregion

        private async Task LoadItemsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                _userDialogs.ShowLoading("Loading...");
                Items.Clear();
                await Task.Delay(1000);
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, 0, 20, _httpClientService.FirmNumber);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
                        foreach (var item in result.Data)
                        {
                            Items.Add(new WarehouseModel
                            {
                                ReferenceId = item.ReferenceId,
                                Name = item.Name,
                                Number = item.Number,
                                City = item.City,
                                Country = item.Country,
                                IsSelected = false
                            });
                        }
                    }
                }

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
                IsBusy = false;
            }
        }

        private async Task LoadMoreItemsAsync()
        {
            if (IsBusy)
                return;
            if (Items.Count < 18)
                return;

            try
            {
                IsBusy = true;

                
                var httpClient = _httpClientService.GetOrCreateHttpClient();
                var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, Items.Count, 20, _httpClientService.FirmNumber);
                if (result.IsSuccess)
                {
                    if (result.Data is not null)
                    {
						_userDialogs.ShowLoading("Loading...");
						foreach (var item in result.Data)
                            Items.Add(new WarehouseModel
                            {
                                ReferenceId = item.ReferenceId,
                                Name = item.Name,
                                Number = item.Number,
                                City = item.City,
                                Country = item.Country,
                                IsSelected = false
                            });
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
                IsBusy = false;
            }
        }

        private void ItemTappedAsync(WarehouseModel item)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;


                Items.ToList().ForEach(x => x.IsSelected = false);

                var selectedItem = Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                if (selectedItem != null)
                    selectedItem.IsSelected = true;

                SelectedWarehouseModel = item;

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

        private async Task NextViewAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(DemandProcessBasketListView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = SelectedWarehouseModel,
                    
                });
            }
            catch (Exception ex)
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                _userDialogs.Alert(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
