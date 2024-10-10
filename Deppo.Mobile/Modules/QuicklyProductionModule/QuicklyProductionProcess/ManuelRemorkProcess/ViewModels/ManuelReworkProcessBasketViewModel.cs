using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;


[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessBasketViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	ReworkBasketModel reworkBasketModel = null!;

	[ObservableProperty]
	ReworkInProductModel? selectedReworkInProductModel;

	public ManuelReworkProcessBasketViewModel(IUserDialogs userDialogs, IServiceProvider serviceProvider)
	{
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;

		Title = "Sepet";

		AddProductTappedCommand = new Command(async () => await AddProductTappedAsync());
		BackCommand = new Command(async () => await BackAsync());


		InProductDecreaseCommand = new Command<ReworkInProductModel>(async (x) => await InProductDecreaseAsync(x));
		InProductIncreaseCommand = new Command<ReworkInProductModel>(async (x) => await InProductIncreaseAsync(x));
	}
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command AddProductTappedCommand { get; }
	public Command BackCommand { get; }
	public Command NextViewCommand { get; }

	public Command InProductIncreaseCommand { get; }
	public Command InProductDecreaseCommand { get; }


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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
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
}
