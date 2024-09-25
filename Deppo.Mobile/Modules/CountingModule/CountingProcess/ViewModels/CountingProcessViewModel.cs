using System;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.ProductCountingProcess.Views;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.Views;

namespace Deppo.Mobile.Modules.CountingModule.CountingProcess.ViewModels;

public partial class CountingProcessViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;
    public CountingProcessViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;

        Title = "Sayım İşlemleri";

        WarehouseCountingProcessCommand = new Command(async () => await WarehouseCountingProcessAsync());
        ProductCountingProcessCommand = new Command(async () => await ProductCountingProcessAsync());
    }

    public Command WarehouseCountingProcessCommand { get; }
	public Command ProductCountingProcessCommand { get; }

	private async Task WarehouseCountingProcessAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            await Shell.Current.GoToAsync($"{nameof(WarehouseCountingWarehouseListView)}");
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

	private async Task ProductCountingProcessAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ProductCountingProductListView)}");
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
