using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

public partial class TransferOutWarehouseListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IWarehouseService _warehouseService;
    private readonly IUserDialogs _userDialogs;

	#region Collections
	public ObservableCollection<WarehouseModel> Items { get; } = new();
    #endregion


    [ObservableProperty]
    WarehouseModel? selectedWarehouseModel;
	
	public TransferOutWarehouseListViewModel(IHttpClientService httpClientService, IWarehouseService warehouseService, IUserDialogs userDialogs)
    {
        _httpClientService = httpClientService;
        _warehouseService = warehouseService;
        _userDialogs = userDialogs;

        Title = "Çıkış Ambarı Seçimi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<WarehouseModel>(ItemTappedAsync);
        NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

    #region Commands
    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }
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
            var result = await _warehouseService.GetObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                search: "",
                skip: 0,
                take: 20,
                externalDb: _httpClientService.ExternalDatabase
            );
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                    {
                        Items.Add(Mapping.Mapper.Map<WarehouseModel>(item));
                    }
                }
            }

            if(_userDialogs.IsHudShowing)
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
			var result = await _warehouseService.GetObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: "",
				skip: 0,
				take: 20,
				externalDb: _httpClientService.ExternalDatabase
			);

			if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
					_userDialogs.Loading("Loading...");
					foreach (var item in result.Data)
                    {
						Items.Add(Mapping.Mapper.Map<WarehouseModel>(item));
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
            IsBusy = false;
        }
    }

    private void ItemTappedAsync(WarehouseModel item)
    {
        if (IsBusy)
            return;

        try
        {
			if (item == SelectedWarehouseModel)
			{
				SelectedWarehouseModel.IsSelected = false;
				SelectedWarehouseModel = null;
			}
			else
			{
				if (SelectedWarehouseModel != null)
				{
					SelectedWarehouseModel.IsSelected = false;
				}

				SelectedWarehouseModel = item;
				SelectedWarehouseModel.IsSelected = true;
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

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        if (SelectedWarehouseModel is null)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(TransferOutBasketView)}", new Dictionary<string, object>
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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if(SelectedWarehouseModel is not null)
            {
                SelectedWarehouseModel.IsSelected = false;
                SelectedWarehouseModel = null;
            }

            await Shell.Current.GoToAsync("..");    
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
