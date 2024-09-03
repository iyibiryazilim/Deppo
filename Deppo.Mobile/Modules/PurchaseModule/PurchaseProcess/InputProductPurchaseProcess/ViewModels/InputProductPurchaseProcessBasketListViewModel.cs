using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Supplier), queryId: nameof(Supplier))]
[QueryProperty(name: nameof(InputProductProcessType), queryId: nameof(InputProductProcessType))]
public partial class InputProductPurchaseProcessBasketListViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private Supplier supplier = null!;

    [ObservableProperty]
    private InputProductProcessType inputProductProcessType;

    public InputProductPurchaseProcessBasketListViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;
        Title = "Sepet Listesi";

        ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
        DeleteItemCommand = new Command<InputProductBasketModel>(async (item) => await DeleteItemAsync(item));
        IncreaseCommand = new Command<InputProductBasketModel>(async (item) => await IncreaseAsync(item));
        DecreaseCommand = new Command<InputProductBasketModel>(async (item) => await DecreaseAsync(item));
        NextViewCommand = new Command(async () => await NextViewAsync());
        BackCommand = new Command(async () => await BackAsync());

        Items.Clear();
    }

    public Command ShowProductViewCommand { get; }
    public Command<InputProductBasketModel> DeleteItemCommand { get; }
    public Command<InputProductBasketModel> IncreaseCommand { get; }
    public Command<InputProductBasketModel> DecreaseCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }

    public ObservableCollection<InputProductBasketModel> Items { get; } = new();

    private async Task ShowProductViewAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseProcessProductListView)}", new Dictionary<string, object>
            {
                {nameof(WarehouseModel), WarehouseModel}
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

    private async Task DeleteItemAsync(InputProductBasketModel item)
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

    private async Task IncreaseAsync(InputProductBasketModel item)
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

    private async Task DecreaseAsync(InputProductBasketModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item.Quantity > 1)
                item.Quantity--;
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
                await Shell.Current.GoToAsync("..");
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