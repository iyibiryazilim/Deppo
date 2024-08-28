using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

[QueryProperty(name: nameof(OutputProductProcessType), queryId: nameof(OutputProductProcessType))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	public OutputProductProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;

		Title = "Sepet Listesi";

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		IncreaseCommand = new Command<OutputProductBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<OutputProductBasketModel>(async (item) => await DecreaseAsync(item));
		DeleteItemCommand = new Command<OutputProductBasketModel>(async (item) => await DeleteItemAsync(item));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	#region Commands
	public Command ShowProductViewCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command DeleteItemCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	#endregion

	#region Properties
	public ContentPage CurrentPage { get; set; } = null!;

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

			if (item is not null)
			{
				if (item.LocTracking == 1)
				{
					OutputProductProcessBasketListView currentPage = CurrentPage as OutputProductProcessBasketListView;
					currentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.HalfExpanded;
				}
				else
				{
					item.Quantity++;
				}
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (Items.Count == 0)
			{
				await _userDialogs.AlertAsync("Sepetinizde ürün bulunmamaktadır.", "Hata", "Tamam");
				return;
			}
			await Shell.Current.GoToAsync($"{nameof(OutputProductProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(OutputProductProcessType)] = OutputProductProcessType
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
}
