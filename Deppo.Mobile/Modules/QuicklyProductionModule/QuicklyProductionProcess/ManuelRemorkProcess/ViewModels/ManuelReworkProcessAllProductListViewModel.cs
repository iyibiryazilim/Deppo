using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
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
    private readonly IVariantService _variantService;

    [ObservableProperty]
    private WarehouseModel inWarehouseModel = null!;

    public ObservableCollection<ProductModel> Items { get; } = new();
    public ObservableCollection<ProductModel> SelectedItems { get; } = new();

	public ObservableCollection<VariantModel> ItemVariants { get; } = new();

	public ObservableCollection<ReworkInProductModel> SelectedReworkInProductModels { get; } = new();

	[ObservableProperty]
    private ProductModel? selectedItem;

    [ObservableProperty]
    public SearchBar searchText;

    public ManuelReworkProcessAllProductListViewModel(IProductService productService, IHttpClientService httpClientService, IServiceProvider serviceProvider, IUserDialogs userDialogs, IVariantService variantService)
    {
        _productService = productService;
        _httpClientService = httpClientService;
        _serviceProvider = serviceProvider;
        _userDialogs = userDialogs;
        _variantService = variantService;

        Title = "Giriş Ürün Listesi";

        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<ProductModel>(async (x) => await ItemTappedAsync(x));
        ConfirmCommand = new Command(async () => await ConfirmAsync());
        BackCommand = new Command(async () => await BackAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

        LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
        VariantTappedCommand = new Command<VariantModel>(async (x) => await VariantTappedAsync(x));
        ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

	public Command LoadMoreVariantItemsCommand { get; }
	public Command VariantTappedCommand { get; }
	public Command ConfirmVariantCommand { get; }

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
                search: SearchText.Text,
                skip: 0,
                take: 20
            );
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<ProductModel>(product);
					var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
					if (matchedItem is not null)
						item.IsSelected = matchedItem.IsSelected;
					else
						item.IsSelected = false;

					Items.Add(item);
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
                search: SearchText.Text,
                skip: Items.Count,
                take: 20
            );
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
					var item = Mapping.Mapper.Map<ProductModel>(product);
					var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
					if (matchedItem is not null)
						item.IsSelected = matchedItem.IsSelected;
					else
						item.IsSelected = false;

					Items.Add(item);
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

            if (SelectedItem.IsSelected)
            {
				SelectedReworkInProductModels.Remove(SelectedReworkInProductModels.FirstOrDefault(x => x.ReferenceId == SelectedItem.ReferenceId));
				SelectedItems.Remove(SelectedItem);
				
				SelectedItem.IsSelected = false;
                SelectedItem = null;
            }
            else
            {
                if (SelectedItem.IsVariant)
                {
                   await LoadVariantItemsAsync(SelectedItem);
                    CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.HalfExpanded;
                }
                else
                {
					ReworkInProductModel reworkInProductModel = new()
					{
						ReferenceId = item.ReferenceId,
						Code = item.Code,
						Name = item.Name,
						MainItemCode = item.Code,
						MainItemName = item.Name,
						MainItemReferenceId = item.ReferenceId,
						Image = item.ImageData,
						InWarehouseModel = InWarehouseModel,
						BrandReferenceId = item.BrandReferenceId,
						BrandCode = item.BrandCode,
						BrandName = item.BrandName,
                        IsSelected = false,
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
						VariantIcon = item.VariantIcon,
						LocTrackingIcon = item.LocTrackingIcon,
						TrackingTypeIcon = item.TrackingTypeIcon,
						InputQuantity = item.LocTracking == 0 ? 1 : 0
					};
                    SelectedReworkInProductModels.Add(reworkInProductModel);

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

            if (SelectedItems.Count == 0)
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

                foreach (var item in SelectedReworkInProductModels)
                {
                    if(!basketViewModel.ReworkBasketModel.ReworkInProducts.Any(x => x.ReferenceId == item.ReferenceId))
                    {
						basketViewModel.ReworkBasketModel.ReworkInProducts.Add(item);
					}
                    
                }


				SelectedReworkInProductModels.Clear();
				SelectedItems.ForEach(x => x.IsSelected = false);
                SelectedItems.Clear();

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
                await Shell.Current.GoToAsync("../..");
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

    private async Task BackAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if (SelectedItems.Count > 0)
            {
                var confirm = await _userDialogs.ConfirmAsync("Seçtiğiniz ürünler silinecektir. Devam etmek istiyor musunuz?", "Bilgilendirme", "Evet", "Hayır");
                if (!confirm)
                    return;

                SearchText.Text = string.Empty;
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
			_userDialogs.Loading("Searching Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _productService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: SearchText.Text,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<ProductModel>(product);
					var matchedItem = SelectedItems.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
					if (matchedItem is not null)
						item.IsSelected = matchedItem.IsSelected;
					else
						item.IsSelected = false;

					Items.Add(item);
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

	private async Task PerformEmptySearchAsync()
	{
		if (string.IsNullOrWhiteSpace(SearchText.Text))
		{
			await PerformSearchAsync();
		}
	}


	private async Task LoadVariantItemsAsync(ProductModel item)
	{
		try
		{
			_userDialogs.Loading("Loading Variant Items...");
			ItemVariants.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantService
								.GetVariants(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, item.ReferenceId, string.Empty, 0, 20);

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;
				
				foreach (var variant in result.Data)
				{
					var obj = Mapping.Mapper.Map<VariantModel>(variant);
					ItemVariants.Add(obj);
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
	}

    private async Task LoadMoreVariantItemsAsync()
    {
		if (ItemVariants.Count < 18)
			return;
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading More Variant Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantService.GetVariants(
                httpClient,
                _httpClientService.FirmNumber, 
                _httpClientService.PeriodNumber, 
                SelectedItem.ReferenceId,
                string.Empty, 
                ItemVariants.Count,
                20
            );

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;

				foreach (var variant in result.Data)
				{
					var obj = Mapping.Mapper.Map<VariantModel>(variant);
					ItemVariants.Add(obj);
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.Loading().Hide();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task VariantTappedAsync(VariantModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			ItemVariants.ToList().ForEach(x => x.IsSelected = false);
			var selectedItem = ItemVariants.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
			if (selectedItem != null)
				selectedItem.IsSelected = true;
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

    private async Task ConfirmVariantAsync()
    {
        if (IsBusy)
			return;
        try
        {
            IsBusy = true;

            var selectedVariantItem = ItemVariants.FirstOrDefault(x => x.IsSelected);
            if(selectedVariantItem is null)
            {
                await _userDialogs.AlertAsync("Lütfen bir varyant seçiniz.", "Uyarı", "Tamam");
                return;
            }

			ReworkInProductModel reworkInProductModel = new()
			{
				ReferenceId = selectedVariantItem.ReferenceId,
				Code = selectedVariantItem.Code,
				Name = selectedVariantItem.Name,
				Image = selectedVariantItem.ImageData,
				MainItemCode = selectedVariantItem.Code,
				MainItemName = selectedVariantItem.Name,
				MainItemReferenceId = selectedVariantItem.ReferenceId,
				InWarehouseModel = InWarehouseModel,
				BrandReferenceId = selectedVariantItem.BrandReferenceId,
				BrandCode = selectedVariantItem.BrandCode,
				BrandName = selectedVariantItem.BrandName,
				IsSelected = false,
				GroupCode = selectedVariantItem.GroupCode,
				UnitsetReferenceId = selectedVariantItem.UnitsetReferenceId,
				UnitsetCode = selectedVariantItem.UnitsetCode,
				UnitsetName = selectedVariantItem.UnitsetName,
				SubUnitsetReferenceId = selectedVariantItem.SubUnitsetReferenceId,
				SubUnitsetCode = selectedVariantItem.SubUnitsetCode,
				SubUnitsetName = selectedVariantItem.SubUnitsetName,
				StockQuantity = selectedVariantItem.StockQuantity,
				VatRate = selectedVariantItem.VatRate,
				IsVariant = true,
				LocTracking = selectedVariantItem.LocTracking,
				TrackingType = selectedVariantItem.TrackingType,
				InputQuantity = selectedVariantItem.LocTracking == 0 ? 1 : 0
			};

            var matchedItem = SelectedReworkInProductModels.FirstOrDefault(x => x.ReferenceId == reworkInProductModel.ReferenceId);
            if (matchedItem is not null)
			{
				await _userDialogs.AlertAsync("Seçtiğiniz varyant zaten bulunmakta.", "Uyarı", "Tamam");
				return;
			}
			SelectedReworkInProductModels.Add(reworkInProductModel);


			SelectedItem.IsSelected = true;
			SelectedItems.Add(SelectedItem);

			CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.Hidden;


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
				SelectedItems.ForEach(x => x.IsSelected = false);
				SelectedItems.Clear();
				SelectedReworkInProductModels.Clear();
                SearchText.Text = string.Empty;

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