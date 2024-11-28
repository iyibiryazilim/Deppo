using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.Views;
using Deppo.Core.BaseModels;
using Deppo.Mobile.Core.Models.ProductModels;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionBOMMenu.ViewModels;

public partial class QuicklyProductionBOMMenuViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IQuicklyBomService _quicklyBomService;

    public ObservableCollection<QuicklyBOMProductModel> Items { get; } = new();

    [ObservableProperty]
    private QuicklyBOMProductModel selectedProduct = null!;

    [ObservableProperty]
    private QuicklyBomProductBasketModel? basketModel = new();

    public Page CurrentPage { get; set; }


    public QuicklyProductionBOMMenuViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IQuicklyBomService quicklyBomService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _quicklyBomService = quicklyBomService;

        Title = "Hızlı Üretim Reçeteleri";

        BackCommand = new Command(async () => await BackAsync());
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<QuicklyBOMProductModel>(async (parameter) => await ItemTappedAsync(parameter));
        ProductionOptionTappedCommand = new Command(async () => await ProductionOptionTappedAsync());
       
        PerformSearchCommand = new Command(async () => await PerformSearchAsync());
        PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ProductionOptionTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }

    public Command PerformSearchCommand { get; }
    public Command PerformEmptySearchCommand { get; }

    [ObservableProperty]
    public SearchBar searchText;

    private async Task ItemTappedAsync(QuicklyBOMProductModel item)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            SelectedProduct = item;

            CurrentPage.FindByName<BottomSheet>("actionBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task ProductionOptionTappedAsync()
    {
        if (IsBusy)
			return;
        try
        {
            IsBusy = true;

			QuicklyBomProductBasketModel quicklyBomProductBasketModel = new();
			quicklyBomProductBasketModel.QuicklyBomProduct = SelectedProduct;
			quicklyBomProductBasketModel.WarehouseName = SelectedProduct.WarehouseName;
			quicklyBomProductBasketModel.WarehouseNumber = SelectedProduct.WarehouseNumber;
			quicklyBomProductBasketModel.QuicklyBomProduct.Amount = SelectedProduct.Amount;
			quicklyBomProductBasketModel.MainAmount = SelectedProduct.Amount;

			CurrentPage.FindByName<BottomSheet>("actionBottomSheet").State = BottomSheetState.Hidden;
            await Task.Delay(300);
			await Shell.Current.GoToAsync($"{nameof(WorkOrderCalcView)}", new Dictionary<string, object>
			{
				[nameof(QuicklyBomProductBasketModel)] = quicklyBomProductBasketModel
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

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyBomService.GetObjectsWorkOrder(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, skip: Items.Count, take: 20,search:SearchText.Text);

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
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

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
            var confirm = await _userDialogs.ConfirmAsync("Çıkmak istediğinize emin misiniz?", "İptal", "Evet", "Hayır");
            if (!confirm)
                return;
            if (SelectedProduct != null)
            {
                SelectedProduct.IsSelected = false;
                SelectedProduct = null;
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
            var result = await _quicklyBomService.GetObjectsWorkOrder(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, search: SearchText.Text);

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
            if (_userDialogs.IsHudShowing)
                _userDialogs.Loading().Hide();

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
            var result = await _quicklyBomService.GetObjectsWorkOrder(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, search: SearchText.Text);

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
}