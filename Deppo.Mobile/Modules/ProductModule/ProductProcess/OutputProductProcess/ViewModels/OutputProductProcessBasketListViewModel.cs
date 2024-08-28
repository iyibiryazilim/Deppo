using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

[QueryProperty(name: nameof(OutputProductProcessType), queryId: nameof(OutputProductProcessType))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductProcessBasketListViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;
	public OutputProductProcessBasketListViewModel(IUserDialogs userDialogs)
	{
		_userDialogs = userDialogs;
		Title = "Sepet Listesi";

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		IncreaseCommand = new Command<OutputProductBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<OutputProductBasketModel>(async (item) => await DecreaseAsync(item));
		DeleteItemCommand = new Command<OutputProductBasketModel>(async (item) => await DeleteItemAsync(item));
		BackCommand = new Command(async () => await BackAsync());
	}

	#region Commands
	public Command ShowProductViewCommand { get; }
	public Command BackCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command DeleteItemCommand { get; }
	#endregion

	#region Properties
	[ObservableProperty]
	OutputProductProcessType outputProductProcessType;
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;
	#endregion

	#region Collections
	public ObservableCollection<OutputProductBasketModel> Items { get; } = new();
	#endregion

	private async Task ShowProductViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(OutputProductProcessProductListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel
			});
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task IncreaseAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			item.Quantity++;
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DecreaseAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item.Quantity > 1)
			{
				item.Quantity--;
			}

		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DeleteItemAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			Items.Remove(item);
		}
		catch (Exception ex)
		{
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
				await Shell.Current.GoToAsync("..");
			}
			else
			{
				await Shell.Current.GoToAsync("..");
			}
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
}
