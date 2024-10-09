using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Core.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;


[QueryProperty(name: nameof(InWarehouseModel), queryId: nameof(InWarehouseModel))]
public partial class ManuelReworkProcessAllProductListViewModel : BaseViewModel
{
	private readonly IProductService _productService;
	private readonly IHttpClientService _httpClientService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;


	[ObservableProperty]
	WarehouseModel inWarehouseModel = null!;

	public ObservableCollection<ProductModel> Items { get; } = new();

	public ObservableCollection<ProductModel> SelectedItems { get; } = new();

	[ObservableProperty]
	ProductModel? selectedItem;

	public ManuelReworkProcessAllProductListViewModel(IProductService productService, IHttpClientService httpClientService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_productService = productService;
		_httpClientService = httpClientService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;

		Title = "Giriş Ürün Listesi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<ProductModel>(async (x) => await ItemTappedAsync(x));
		ConfirmCommand = new Command(async () => await ConfirmAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public Page CurrentPage { get; set; } = null!;

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command ConfirmCommand { get; }
	public Command BackCommand { get; }

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading...");
			await Task.Delay(1000);
			Items.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _productService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: string.Empty,
				skip: 0,
				take: 20
			);
			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<ProductModel>(item));
				}
			}

			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading More Items...");
			
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _productService.GetObjects(
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

				foreach (var item in result.Data)
				{
					Items.Add(Mapping.Mapper.Map<ProductModel>(item));
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

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

	private async Task ItemTappedAsync(ProductModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedItem = item;

			if(SelectedItem.IsSelected)
			{
				SelectedItems.Remove(SelectedItem);
				SelectedItem.IsSelected = false;
				SelectedItem = null;
			}
			else
			{
				if(SelectedItem.IsVariant)
				{
					// Variant logic comes here
				}
				else
				{
					SelectedItem.IsSelected = true;
					SelectedItems.Add(SelectedItem);
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

	private async Task ConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(SelectedItems.Count == 0)
			{
				await _userDialogs.AlertAsync("Lütfen en az bir ürün seçiniz.", "Uyarı", "Tamam");
				return;
			}
			else
			{
				var confirm = await _userDialogs.ConfirmAsync("Seçtiğiniz ürünler sepete eklenecektir. Devam etmek istiyor musunuz?", "Bilgilendirme", "Evet", "Hayır");
				if (!confirm)
					return;

				var basketViewModel = _serviceProvider.GetRequiredService<ManuelReworkProcessBasketViewModel>();
				_userDialogs.Loading("Seçilen ürünler sepete ekleniyor.");

				foreach(var item in SelectedItems)
				{
					ReworkInProductModel reworkInProductModel = new()
					{
						Code = item.Code,
						Name = item.Name,
						Image = item.Image,
						InWarehouseModel = InWarehouseModel,
						ReferenceId = item.ReferenceId,
						BrandReferenceId = item.BrandReferenceId,
						BrandCode = item.BrandCode,
						BrandName = item.BrandName,
						GroupCode = item.GroupCode,
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetCode = item.UnitsetCode,
						UnitsetName = item.UnitsetName,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						SubUnitsetCode = item.SubUnitsetCode,
						SubUnitsetName = item.SubUnitsetName,
						StockQuantity = item.StockQuantity,
						VatRate = item.VatRate,
						IsVariant = item.IsVariant,
						LocTracking = item.LocTracking,
						TrackingType = item.TrackingType,
					    InputQuantity = item.LocTracking == 0 ? 1 : 0
					};

					basketViewModel.ReworkBasketModel.ReworkInProducts.Add(reworkInProductModel);
				}

				SelectedItems.ForEach(x => x.IsSelected = false);
				SelectedItems.Clear();

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();
				await Shell.Current.GoToAsync("../..");
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

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(SelectedItems.Count > 0)
			{
				var confirm = await _userDialogs.ConfirmAsync("Seçtiğiniz ürünler silinecektir. Devam etmek istiyor musunuz?", "Bilgilendirme", "Evet", "Hayır");
				if (!confirm)
					return;

				SelectedItems.ForEach(x => x.IsSelected = false);
				SelectedItems.Clear();
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
