using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Data.Async.Helpers;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;

public partial class InputOutsourceTransferV2WarehouseListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IOutsourceService _outsourceService;

	public ObservableCollection<WarehouseModel> Items { get; } = new();

	[ObservableProperty]
	WarehouseModel? selectedWarehouseModel;

	public InputOutsourceTransferV2WarehouseListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IOutsourceService outsourceService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_outsourceService = outsourceService;

		Title = "Fason Ambarı Seçimi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseModel>(async (warehouseModel) => await ItemTappedAsync(warehouseModel));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command BackCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command NextViewCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Load Items...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _outsourceService.GetOutsourceWarehousesAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: string.Empty,
				skip: 0,
				take: 20
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<WarehouseModel>(item);
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

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
			var result = await _outsourceService.GetOutsourceWarehousesAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: string.Empty,
				skip: Items.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				_userDialogs.Loading("Load More Items...");

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<WarehouseModel>(item);
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

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ItemTappedAsync(WarehouseModel warehouseModel)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(SelectedWarehouseModel == warehouseModel)
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
				SelectedWarehouseModel = warehouseModel;
				SelectedWarehouseModel.IsSelected = true;
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
	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		if (SelectedWarehouseModel is null)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferV2SupplierListViewModel)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = SelectedWarehouseModel
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
