using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;


[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessBasketViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	ReworkBasketModel reworkBasketModel = null!;

	[ObservableProperty]
	ReworkInProductModel? selectedReworkInProductModel;

	public ManuelReworkProcessBasketViewModel(IHttpClientService httpClientService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_locationTransactionService = locationTransactionService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;

		Title = "Sepet";

		IncreaseCommand = new Command(async () => await IncreaseAsync());
		DecreaseCommand = new Command(async () => await DecreaseAsync());
		AddProductTappedCommand = new Command(async () => await AddProductTappedAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());


		InProductDecreaseCommand = new Command<ReworkInProductModel>(async (x) => await InProductDecreaseAsync(x));
		InProductIncreaseCommand = new Command<ReworkInProductModel>(async (x) => await InProductIncreaseAsync(x));
		InProductDeleteCommand = new Command<ReworkInProductModel>(async (x) => await InProductDeleteAsync(x));
	}
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command AddProductTappedCommand { get; }
	public Command BackCommand { get; }
	public Command NextViewCommand { get; }

	public Command InProductIncreaseCommand { get; }
	public Command InProductDecreaseCommand { get; }
	public Command InProductDeleteCommand { get; }

	private async Task IncreaseAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(ReworkBasketModel.ReworkOutProductModel.StockQuantity > ReworkBasketModel.ReworkOutProductModel.OutputQuantity)
			{
				if(ReworkBasketModel.ReworkOutProductModel.LocTracking == 1 && ReworkBasketModel.ReworkOutProductModel.TrackingType == 0)
				{

				}
				else if (ReworkBasketModel.ReworkOutProductModel.LocTracking == 0 && ReworkBasketModel.ReworkOutProductModel.TrackingType == 0)
				{
					ReworkBasketModel.ReworkOutProductModel.OutputQuantity += 1;
				}
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

	private async Task DecreaseAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (ReworkBasketModel.ReworkOutProductModel.OutputQuantity > 0)
			{
				if (ReworkBasketModel.ReworkOutProductModel.LocTracking == 1 && ReworkBasketModel.ReworkOutProductModel.TrackingType == 0)
				{

				}
				else if (ReworkBasketModel.ReworkOutProductModel.LocTracking == 0 && ReworkBasketModel.ReworkOutProductModel.TrackingType == 0)
				{
					ReworkBasketModel.ReworkOutProductModel.OutputQuantity -= 1;
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(ReworkBasketModel)] = ReworkBasketModel
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

			var confirm = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			if (ReworkBasketModel.ReworkInProducts.Any())
			{
				foreach (var item in ReworkBasketModel.ReworkInProducts)
				{
					if (item.Details.Any())
						item.Details.Clear();
				}

				ReworkBasketModel.ReworkInProducts.Clear();
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

	private async Task InProductIncreaseAsync(ReworkInProductModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedReworkInProductModel = item;

			var locationViewModel = _serviceProvider.GetRequiredService<ManuelReworkProcessBasketLocationListViewModel>();

			if(SelectedReworkInProductModel.LocTracking == 1 && SelectedReworkInProductModel.TrackingType == 0)
			{
				await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessBasketLocationListView)}", new Dictionary<string, object>
				{
					["SelectedReworkInProductModel"] = SelectedReworkInProductModel
				});

				await locationViewModel.LoadSelectedItemsAsync();
			}
			else if(SelectedReworkInProductModel.LocTracking == 0 && SelectedReworkInProductModel.TrackingType == 0)
			{
				SelectedReworkInProductModel.InputQuantity += 1;
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

	private async Task InProductDecreaseAsync(ReworkInProductModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(item.InputQuantity > 0)
			{
				SelectedReworkInProductModel = item;

				if (SelectedReworkInProductModel.LocTracking == 1 && SelectedReworkInProductModel.TrackingType == 0)
				{
					await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessBasketLocationListView)}", new Dictionary<string, object>
					{
						["SelectedReworkInProductModel"] = SelectedReworkInProductModel
					});
				}
				else if (SelectedReworkInProductModel.LocTracking == 0 && SelectedReworkInProductModel.TrackingType == 0)
				{
					SelectedReworkInProductModel.InputQuantity -= 1;
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

	private async Task InProductDeleteAsync(ReworkInProductModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectedReworkInProductModel = item;

			var confirm = await _userDialogs.ConfirmAsync("Ürünü silmek istediğinize emin misiniz?", "Sil", "Evet", "Hayır");
			if(!confirm)
				return;

			ReworkBasketModel.ReworkInProducts.Remove(SelectedReworkInProductModel);

			if (SelectedReworkInProductModel.Details.Any())
			{
				SelectedReworkInProductModel.Details.Clear();
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

	private async Task AddProductTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessInWarehouseListView)}");
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

	public async Task ClearPageAsync()
	{
		try
		{
			await Task.Run(() =>
			{
				if(SelectedReworkInProductModel is not null)
				{
					SelectedReworkInProductModel.Details.Clear();
					SelectedReworkInProductModel = null;
				}
				if(ReworkBasketModel is not null)
				{
					ReworkBasketModel = null;
				}
			});
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}
