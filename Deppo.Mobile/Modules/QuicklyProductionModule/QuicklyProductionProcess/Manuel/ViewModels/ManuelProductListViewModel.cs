using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.ViewModels;

public partial class ManuelProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISeriLotTransactionService _serilotTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly IQuicklyBomService _quicklyBomService;
    private readonly IVariantService _variantService;

    public ObservableCollection<QuicklyBOMProductModel> Items { get; } = new();
    public ObservableCollection<VariantModel> ItemVariants { get; } = new();

    [ObservableProperty]
    private QuicklyBOMProductModel? selectedProduct;

    [ObservableProperty]
    private QuicklyBomProductBasketModel? basketModel = new();


	[ObservableProperty]
	public SearchBar searchText;


    public ManuelProductListViewModel(IHttpClientService httpClientService, ISeriLotTransactionService serilotTransactionService, IUserDialogs userDialogs, IQuicklyBomService quicklyBomService, IVariantService variantService)
    {
        _httpClientService = httpClientService;
        _serilotTransactionService = serilotTransactionService;
        _userDialogs = userDialogs;
        _quicklyBomService = quicklyBomService;
        _variantService = variantService;

        Title = "Ürün Listesi";

        BackCommand = new Command(async () => await BackAsync());
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<QuicklyBOMProductModel>(async (item) => await ItemTappedAsync(item));
        NextViewCommand = new Command(async () => await NextViewAsync());
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

        LoadVariantItemsCommand = new Command<QuicklyBOMProductModel>(async (item) => await LoadVariantItemsAsync(item));
        LoadMoreVariantItemsCommand = new Command(async () => await LoadMoreVariantItemsAsync());
        VariantTappedCommand = new Command<VariantModel>(async (item) => await VariantTappedAsync(item));
        ConfirmVariantCommand = new Command(async () => await ConfirmVariantAsync());
    }

	public Page CurrentPage { get; set; }

	public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }
    public Command NextViewCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

	public Command LoadVariantItemsCommand { get; }
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

			_userDialogs.Loading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _quicklyBomService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, search: SearchText.Text);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<QuicklyBOMProductModel>(product);

					Items.Add(item);
					/* Items.Add(new QuicklyBOMProductModel
                     {
                         ReferenceId = (int)item.ProductReferenceId,
                         Code = item.ProductCode,
                         Name = item.ProductName,
                         UnitsetReferenceId = item.UnitsetReferenceId,
                         UnitsetCode = item.UnitsetCode,
                         UnitsetName = item.UnitsetName,
                         SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                         SubUnitsetCode = item.SubUnitsetCode,
                         SubUnitsetName = item.SubUnitsetName,
                         StockQuantity = item.StockQuantity,
                         LocTracking = item.LocTracking,
                         IsVariant = item.IsVariant,
                         TrackingType = item.TrackingType,
                         IsSelected = false,
                         LocTrackingIcon = product.LocTrackingIcon,
                         VariantIcon = product.VariantIcon,
                         TrackingTypeIcon = product.TrackingTypeIcon,
                     });*/
				}
			}

			_userDialogs.Loading().Hide();
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
	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Loading...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _quicklyBomService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, skip: Items.Count, take: 20, search: SearchText.Text);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<QuicklyBOMProductModel>(product);

					Items.Add(item);
					/* Items.Add(new QuicklyBOMProductModel
                     {
                         ReferenceId = item.ProductReferenceId,
                         Code = item.ProductCode,
                         Name = item.ProductName,
                         UnitsetReferenceId = item.UnitsetReferenceId,
                         UnitsetCode = item.UnitsetCode,
                         UnitsetName = item.UnitsetName,
                         SubUnitsetReferenceId = item.SubUnitsetReferenceId,
                         SubUnitsetCode = item.SubUnitsetCode,
                         SubUnitsetName = item.SubUnitsetName,
                         StockQuantity = item.StockQuantity,
                         LocTracking = item.LocTracking,
                         IsVariant = item.IsVariant,
                         TrackingType = item.TrackingType,
                         IsSelected = false,
                         LocTrackingIcon = product.LocTrackingIcon,
                         VariantIcon = product.VariantIcon,
                         TrackingTypeIcon = product.TrackingTypeIcon,
                     });*/
				}
			}

			_userDialogs.Loading().Hide();
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
	private async Task ItemTappedAsync(QuicklyBOMProductModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedProduct = item;

			if (!item.IsSelected)
			{
				if (item.IsVariant)
				{
					await LoadVariantItemsAsync(item);
					CurrentPage.FindByName<BottomSheet>("variantBottomSheet").State = BottomSheetState.HalfExpanded;
				}
				else
				{
                    Items.ToList().ForEach(x => x.IsSelected = false);
					item.IsSelected = true;
				}
			}
			else
			{
				item.IsSelected = false;
				SelectedProduct = null;
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

            if (SelectedProduct is null)
            {
                await _userDialogs.AlertAsync("Lütfen bir ürün seçiniz.", "Hata", "Tamam");
                return;
            }

            BasketModel.QuicklyBomProduct = SelectedProduct;
            await Shell.Current.GoToAsync($"{nameof(ManuelCalcInWarehouseListView)}", new Dictionary<string, object>
            {
                [nameof(QuicklyBomProductBasketModel)] = BasketModel
            });
        }
        catch (System.Exception ex)
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
            var confirm = await _userDialogs.ConfirmAsync("İşlemi iptal etmek istediğinize emin misiniz?", "İptal", "Evet", "Hayır");
            if (!confirm)
                return;
            if (SelectedProduct != null)
            {
                SelectedProduct.IsSelected = false;
                SelectedProduct = null;
                if(BasketModel is not null)
                {
                    BasketModel.WarehouseNumber = default;
                    BasketModel.WarehouseName = string.Empty;
                    BasketModel.BOMQuantity = default;
                    if(BasketModel.QuicklyBomProduct is not null)
                    BasketModel.QuicklyBomProduct = null;
                }
            }

            await Shell.Current.GoToAsync($"..");
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
            var result = await _quicklyBomService.GetObjects(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, search: SearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var product in result.Data)
                {
                    var item = Mapping.Mapper.Map<QuicklyBOMProductModel>(product);

                    Items.Add(item);
                  
                }
            }

            _userDialogs.Loading().Hide();

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

    private async Task LoadVariantItemsAsync(QuicklyBOMProductModel quicklyBOMProductModel)
    {
        try
        {
			_userDialogs.Loading("Loading Variant Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _variantService.GetVariants(
                httpClient: httpClient, 
                firmNumber: _httpClientService.FirmNumber, 
                periodNumber: _httpClientService.PeriodNumber, 
                productReferenceId: quicklyBOMProductModel.ReferenceId, 
                search: string.Empty, 
                skip: 0, 
                take: 20
            );

			if (result.IsSuccess)
			{
				if (result.Data == null)
					return;
				ItemVariants.Clear();
				foreach (var variant in result.Data)
				{
					var obj = Mapping.Mapper.Map<VariantModel>(variant);
					ItemVariants.Add(obj);
				}
			}

			if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
		}
        catch (Exception ex)
        {
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

			_userDialogs.Loading("Loading More Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _variantService.GetVariants(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedProduct.ReferenceId,
				search: string.Empty,
				skip: ItemVariants.Count,
				take: 20
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
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
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

            var selectedItem = ItemVariants.FirstOrDefault(x => x.IsSelected);
            if (selectedItem is null)
                return;

            var selectedProductWithVariant = new QuicklyBOMProductModel
            {
                ReferenceId = selectedItem.ReferenceId,
                Code = selectedItem.Code,
                Name = selectedItem.Name,
                MainItemReferenceId = selectedItem.ProductReferenceId,
                MainItemCode = selectedItem.ProductCode,
                MainItemName = selectedItem.ProductName,
                VatRate = selectedItem.VatRate,
                UnitsetReferenceId = selectedItem.UnitsetReferenceId,
                UnitsetCode = selectedItem.UnitsetCode,
                UnitsetName = selectedItem.UnitsetName,
                SubUnitsetReferenceId = selectedItem.SubUnitsetReferenceId,
                SubUnitsetCode = selectedItem.SubUnitsetCode,
                SubUnitsetName = selectedItem.SubUnitsetName,
                IsVariant = true,
                TrackingType = selectedItem.TrackingType,
                LocTracking = selectedItem.LocTracking,
                BrandReferenceId = selectedItem.BrandReferenceId,
                BrandCode = selectedItem.BrandCode,
                BrandName = selectedItem.BrandName,
                StockQuantity = selectedItem.StockQuantity,
                GroupCode = selectedItem.GroupCode,
                IsSelected = true,
            };

            SelectedProduct = selectedProductWithVariant;
            Items.FirstOrDefault(x => x.ReferenceId == SelectedProduct.ReferenceId).IsSelected = true;
            
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
                // Clear all items
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