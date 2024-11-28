using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using Kotlin.Contracts;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(InputProductBasketModel), queryId: nameof(InputProductBasketModel))]
public partial class InputProductProcessBasketSeriLotListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IServiceProvider _serviceProvider;
	private readonly ISeriLotService _seriLotService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	InputProductBasketModel inputProductBasketModel = null!;

	public ObservableCollection<SeriLotModel> Items { get; } = new();
	public ObservableCollection<SeriLotModel> SelectedItems { get; } = new();

	public InputProductProcessBasketSeriLotListViewModel(
		IHttpClientService httpClientService,
		IUserDialogs userDialogs,
		IServiceProvider serviceProvider,
		ISeriLotService seriLotService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_seriLotService = seriLotService;

		
		IncreaseCommand = new Command<SeriLotModel>(async (seriLotModel) => await IncreaseAsync(seriLotModel));
		DecreaseCommand = new Command<SeriLotModel>(async (seriLotModel) => await DecreaseAsync(seriLotModel));
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		CancelCommand = new Command(async () => await CancelAsync());

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
	}


	public Page CurrentPage { get; set; } = null!;

	public Command<Entry> PerformSearchCommand { get; }
	public Command<SeriLotModel> IncreaseCommand { get; }
	public Command<SeriLotModel> DecreaseCommand { get; }
	public Command ConfirmCommand { get; }
	public Command CancelCommand { get; }

	#region SeriLot BottomSheet Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ShowSeriLotsCommand { get; }
	public Command CloseSeriLotsCommand { get; }
	public Command<SeriLotModel> ItemTappedCommand { get; }
	public Command ConfirmSeriLotsCommand { get; }

	#endregion

	private async Task IncreaseAsync(SeriLotModel seriLotModel)
	{
		try
		{
			IsBusy = true;

			seriLotModel.InputQuantity++;
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DecreaseAsync(SeriLotModel seriLotModel)
	{
		try
		{
			IsBusy = true;

			if (seriLotModel.InputQuantity > 0)
				seriLotModel.InputQuantity--;

		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			
			if (SelectedItems.Count > 0)
			{
				var totalQuantity = SelectedItems.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);
				// Stok yeri takipli deðilse ana ürünün miktarýný arttýr
				if (InputProductBasketModel.LocTracking != 0)
				{
					var previousViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>(); ;
					previousViewModel.SelectedInputProductBasketModel.Quantity = totalQuantity;
				}else
				{
					var locationViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketLocationListViewModel>();
					locationViewModel.SelectedItem.InputQuantity = totalQuantity;
				}
			}

			await Shell.Current.GoToAsync("..");
			_userDialogs.HideHud();
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

	private async Task CancelAsync()
	{
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
			await _userDialogs.AlertAsync(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}


	private async Task ShowSeriLotsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			await LoadItemsAsync();
			CurrentPage.FindByName<BottomSheet>("seriLotBottomSheet").State = BottomSheetState.HalfExpanded;
		}
		catch(Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			 _userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task ItemTappedAsync(SeriLotModel seriLotModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if(seriLotModel.IsSelected)
			{
				Items.FirstOrDefault(x => x.ReferenceId == seriLotModel.ReferenceId).IsSelected = false;
			}else
			{
				Items.FirstOrDefault(x => x.ReferenceId == seriLotModel.ReferenceId).IsSelected = true;
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

	private async Task CloseSeriLotsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			await Task.Delay(500);
			CurrentPage.FindByName<BottomSheet>("seriLotBottomSheet").State = BottomSheetState.Hidden;
			_userDialogs.HideHud();
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

	private async Task LoadItemsAsync()
	{
		try
		{
			IsBusy = true;
			Items.Clear();

			_userDialogs.ShowLoading("Loading...");
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _seriLotService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				skip: 0,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
						Items.Add(Mapping.Mapper.Map<SeriLotModel>(item));
				}
			}

			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadMoreItemsAsync()
	{
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _seriLotService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel.Number,
				skip: Items.Count,
				take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
						Items.Add(Mapping.Mapper.Map<SeriLotModel>(item));
				}
			}

			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	

}
