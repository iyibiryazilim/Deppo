using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace Deppo.Mobile.Modules.CameraModule.ViewModels;

public partial class CameraReaderViewModel : BaseViewModel
{
    private readonly IUserDialogs _userDialogs;
    public CameraReaderViewModel(IUserDialogs userDialogs)
    {
        _userDialogs = userDialogs;
        
        BackCommand = new Command(async () => await BackAsync());
        CameraDetectedCommand = new Command<BarcodeDetectionEventArgs>(async (e) => await CameraDetectedAsync(e));
    }

    public CameraBarcodeReaderView BarcodeReader { get; set; } = null!;

    public Command BackCommand { get;}
    public Command CameraDetectedCommand { get; }

    private async Task CameraDetectedAsync(BarcodeDetectionEventArgs e)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

			var first = e.Results?.FirstOrDefault();

			if (first is null)
			{
				await _userDialogs.AlertAsync("Barkod Bulunamadı", "Hata", "Tamam");
				return;
			}

			await _userDialogs.AlertAsync("Barkod Bulundu", first.Value, "Tamam");
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

            await Shell.Current.GoToAsync("..");
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
}
