using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.CountingModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.ViewModels;


[QueryProperty(name: nameof(ProductCountingWarehouseModel), queryId: nameof(ProductCountingWarehouseModel))]
[QueryProperty(name: nameof(ProductCountingBasketModel), queryId: nameof(ProductCountingBasketModel))]
public partial class ProductCountingLocationListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly ILocationService _locationService;

	[ObservableProperty]
	LocationModel? selectedLocation;

	[ObservableProperty]
    ProductCountingBasketModel productCountingBasketModel = null!;

    [ObservableProperty]
    ProductCountingWarehouseModel productCountingWarehouseModel = null!;

	public ObservableCollection<LocationModel> Items { get; } = new();
	public ProductCountingLocationListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationService locationService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationService = locationService;

		Title = "Raf Seçimi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<LocationModel>(async (item) => await ItemTappedAsync(item));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

    }

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

    [ObservableProperty]
    public SearchBar searchText;
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
			var result = await _locationService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: ProductCountingWarehouseModel.Number,
				productReferenceId: ProductCountingBasketModel.IsVariant ? ProductCountingBasketModel.MainItemReferenceId : ProductCountingBasketModel.ItemReferenceId,
				skip: 0,
				take: 20,
				search:SearchText.Text
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;
				
				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<LocationModel>(item);
					obj.IsSelected = (SelectedLocation != null && SelectedLocation.ReferenceId == obj.ReferenceId) ? SelectedLocation.IsSelected : false;
					Items.Add(obj);
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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		if (Items.Count < 18)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading More Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: ProductCountingWarehouseModel.Number,
                productReferenceId: ProductCountingBasketModel.IsVariant ? ProductCountingBasketModel.MainItemReferenceId : ProductCountingBasketModel.ItemReferenceId,
                skip: Items.Count,
				take: 20,
                search: SearchText.Text
            );

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<LocationModel>(item);
					obj.IsSelected = (SelectedLocation != null && SelectedLocation.ReferenceId == obj.ReferenceId) ? SelectedLocation.IsSelected : false;
					Items.Add(obj);
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

	private async Task ItemTappedAsync(LocationModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item == SelectedLocation)
			{
				SelectedLocation.IsSelected = false;
				SelectedLocation = null;
			}
			else
			{
				if (SelectedLocation != null)
				{
					SelectedLocation.IsSelected = false;
				}
				
				SelectedLocation = item;
				SelectedLocation.IsSelected = true;

				Items.Where(x => x.ReferenceId != item.ReferenceId).ToList().ForEach(x => x.IsSelected = false);
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;

		if (SelectedLocation is null)
			return;
		try
		{
			IsBusy = true;

			ProductCountingBasketModel.OutputQuantity = SelectedLocation.StockQuantity;
            ProductCountingBasketModel.StockQuantity = SelectedLocation.StockQuantity;
            await Shell.Current.GoToAsync($"{nameof(ProductCountingBasketView)}", new Dictionary<string, object>
			{
				[nameof(LocationModel)] = SelectedLocation,
				[nameof(ProductCountingWarehouseModel)] = ProductCountingWarehouseModel,
				[nameof(ProductCountingBasketModel)] = ProductCountingBasketModel
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

			if(SelectedLocation is not null)
			{
				SelectedLocation.IsSelected = false;
				SelectedLocation = null;
			}

			SearchText.Text = string.Empty;

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
    private async Task PerformSearchAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(SearchText.Text))
            {
                await LoadItemsAsync();
                SearchText.Unfocus();
                return;
            }
            IsBusy = true;
            Items.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                warehouseNumber: ProductCountingWarehouseModel.Number,
                productReferenceId: ProductCountingBasketModel.IsVariant ? ProductCountingBasketModel.MainItemReferenceId : ProductCountingBasketModel.ItemReferenceId,
                skip: 0,
                take: 20,
                search: SearchText.Text
            );

            if (result.IsSuccess)
            {
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<LocationModel>(item);
					obj.IsSelected = (SelectedLocation != null && SelectedLocation.ReferenceId == obj.ReferenceId) ? SelectedLocation.IsSelected : false;
					Items.Add(obj);
				}                
            }
        }
        catch (System.Exception ex)
        {
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PerformEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText.Text))
        {
            await PerformSearchAsync();
        }
    }

}
