using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;


[QueryProperty(name: nameof(InputOutsourceTransferV2BasketModel), queryId: nameof(InputOutsourceTransferV2BasketModel))]
public partial class InputOutsourceTransferV2BasketViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;
	public InputOutsourceTransferV2BasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;

		Title = "Fason Kabul Sepeti";


		LoadPageCommand = new Command(async () => await LoadPageAsync());
		IncreaseCommand = new Command(async () => await IncreaseAsync());
		DecreaseCommand = new Command(async () => await DecreaseAsync());
		QuantityTappedCommand = new Command(async () => await QuantityTappedAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}
	public Page CurrentPage { get; set; } = null!;
	public Command LoadPageCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command QuantityTappedCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	[ObservableProperty]
	InputOutsourceTransferV2BasketModel inputOutsourceTransferV2BasketModel;

	private async Task LoadPageAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Yükleniyor...");
			//InputOutsourceTransferV2BasketModel.SubProducts.Clear();
			await Task.Delay(1000);


			var httpClient = _httpClientService.GetOrCreateHttpClient();


			// TODO: Alt ürünlerin yüklenmesi 

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

	private async Task IncreaseAsync()
	{
		if (IsBusy)
			return;
		if (InputOutsourceTransferV2BasketModel is null || InputOutsourceTransferV2BasketModel?.InputOutsourceTransferMainProductModel is null)
			return;

		try
		{
			IsBusy = true;

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1)
			{
				// TODO: Raf yerleri açılacak
				await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferV2MainProductLocationListView)}", new Dictionary<string, object>
				{
					[nameof(InputOutsourceTransferV2BasketModel)] = InputOutsourceTransferV2BasketModel
				});
			}
			else
			{
				InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity += 1;

				// TODO: Sarf Malzemeler çarpan kadar miktarı arttırılacak
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

	private async Task DecreaseAsync()
	{
		if (IsBusy)
			return;

		if (InputOutsourceTransferV2BasketModel is null || InputOutsourceTransferV2BasketModel?.InputOutsourceTransferMainProductModel is null)
			return;
		try
		{
			IsBusy = true;

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1 && InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity == 0)
				return;

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 0 && InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity == 1)
				return;

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1)
			{
				// TODO: Raf yerleri açılacak

				await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferV2MainProductLocationListView)}", new Dictionary<string, object>
				{
					[nameof(InputOutsourceTransferV2BasketModel)] = InputOutsourceTransferV2BasketModel
				});
			}
			else
			{
				// TODO: Sarf Malzemeler çarpan kadar miktarı kadar azaltılacak

				InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity -= 1;
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

	private async Task QuantityTappedAsync()
	{
		if (IsBusy)
			return;
		if (InputOutsourceTransferV2BasketModel is null || InputOutsourceTransferV2BasketModel?.InputOutsourceTransferMainProductModel is null)
			return;

		if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1) // Malzeme raf takipli ise bu fonksiyon çalışmamalı
			return;

		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity.ToString(),
				keyboard: Keyboard.Numeric
			);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 0 && quantity < 1)
			{
				await _userDialogs.AlertAsync("Girilen miktar 1'den küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}


			InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity = quantity;
			// TODO: Sarf Malzemelerin quantityleri buradaki sayı kadar çarpılacak 

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

			Console.WriteLine(InputOutsourceTransferV2BasketModel);

			await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferV2FormView)}");
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
