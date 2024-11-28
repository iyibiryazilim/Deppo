using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;

public partial class ManuelReworkProcessOutWarehouseListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IWarehouseService _warehouseService;

	public ObservableCollection<WarehouseModel> Items { get; } = new();

	[ObservableProperty]
	WarehouseModel? selectedWarehouseModel;

	public ManuelReworkProcessOutWarehouseListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IWarehouseService warehouseService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_warehouseService = warehouseService;

		Title = "Çıkış Ambarı Seçimi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<WarehouseModel>(async (x) => await ItemTappedAsync(x));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command NextViewCommand { get; }
	public Command<WarehouseModel> ItemTappedCommand { get; }
	public Command BackCommand { get; }

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

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading More Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, Items.Count, 20, _httpClientService.FirmNumber);
			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
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

	private async Task ItemTappedAsync(WarehouseModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

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
		if (SelectedWarehouseModel is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			ReworkBasketModel reworkBasketModel = new();
			reworkBasketModel.OutWarehouseModel = SelectedWarehouseModel;

			await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessWarehouseTotalListView)}", new Dictionary<string, object>
			{
				[nameof(ReworkBasketModel)] = reworkBasketModel
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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("İşlemi iptal etmek istediğinize emin misiniz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			await ClearPageAsync();
			await Shell.Current.GoToAsync("..");
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

	public async Task ClearPageAsync()
	{
		try
		{
			await Task.Run(() =>
			{
				if (SelectedWarehouseModel is not null)
				{
					SelectedWarehouseModel.IsSelected = false;
					SelectedWarehouseModel = null;
				}
			});
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

}
