using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;

using System;
using System.Collections.Generic;

using System.Collections.ObjectModel;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Deppo.Core.Models;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.VirmanProductProcess.Views;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.QuicklyModels;
using Deppo.Core.BaseModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.Manuel.Views;
using Deppo.Mobile.Core.Models.QuicklyModels.BasketModels;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.Views;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.WorkOrder.ViewModels;

public partial class WorkOrderProductListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IQuicklyBomService _quicklyBomService;

    public ObservableCollection<QuicklyBOMProductModel> Items { get; } = new();

    [ObservableProperty]
    private QuicklyBOMProductModel? selectedProduct;

    [ObservableProperty]
    private QuicklyBomProductBasketModel? basketModel = new();

    public Page CurrentPage { get; set; }

    public WorkOrderProductListViewModel(IHttpClientService httpClientService, ISeriLotTransactionService serilotTransactionService, IUserDialogs userDialogs, IQuicklyBomService quicklyBomService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _quicklyBomService = quicklyBomService;

        Title = "Reçete Ürün Listesi";

        BackCommand = new Command(async () => await BackAsync());
        LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
        ItemTappedCommand = new Command<QuicklyBOMProductModel>(async (parameter) => await ItemTappedAsync(parameter));
        NextViewCommand = new Command(async () => await NextViewAsync());




    }

    public Command LoadItemsCommand { get; }
    public Command LoadMoreItemsCommand { get; }
    public Command ItemTappedCommand { get; }
    public Command ConfirmCommand { get; }
    public Command BackCommand { get; }
    public Command NextViewCommand { get; }

    private async Task NextViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            BasketModel.QuicklyBomProduct = SelectedProduct;
            BasketModel.WarehouseName = SelectedProduct.WarehouseName;
            BasketModel.WarehouseNumber = SelectedProduct.WarehouseNumber;
            BasketModel.QuicklyBomProduct.Amount = SelectedProduct.Amount;
            await Shell.Current.GoToAsync($"{nameof(WorkOrderCalcView)}", new Dictionary<string, object>
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

    private async Task ItemTappedAsync(QuicklyBOMProductModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item is not null)
            {
                if (item == SelectedProduct)
                {
                    SelectedProduct.IsSelected = false;
                    SelectedProduct = null;
                }
                else
                {
                    if (SelectedProduct != null)
                    {
                        SelectedProduct.IsSelected = false;
                    }

                    SelectedProduct = item;
                    SelectedProduct.IsSelected = true;

                    var selectedItem = Items.FirstOrDefault(x => x.ReferenceId == item.ReferenceId);
                    if (selectedItem is not null)
                    {
                        selectedItem.IsSelected = true;
                    }
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

    private async Task LoadMoreItemsAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _quicklyBomService.GetObjectsWorkOrder(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber, skip: Items.Count, take: 20);

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
            var result = await _quicklyBomService.GetObjectsWorkOrder(httpClient, firmNumber: _httpClientService.FirmNumber, periodNumber: _httpClientService.PeriodNumber);

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
}