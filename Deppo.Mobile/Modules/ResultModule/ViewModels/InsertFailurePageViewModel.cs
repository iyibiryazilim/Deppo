using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using Xamarin.Google.Crypto.Tink.Signature;

namespace Deppo.Mobile.Modules.ResultModule.ViewModels;


[QueryProperty(name: nameof(ResultModel), queryId: nameof(ResultModel))]
public partial class InsertFailurePageViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	ResultModel resultModel = null!;

	public InsertFailurePageViewModel(IUserDialogs userDialogs)
	{
		_userDialogs = userDialogs;

		BackCommand = new Command(async () => await BackAsync());
		ErrorTappedCommand = new Command(async () => await ErrorTappedAsync());
	}

	public Page CurrentPage { get; set; } = null!;

    public Command ErrorTappedCommand { get; }
	public Command BackCommand { get; }
	

	async Task ErrorTappedAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("errorBottomSheet").State = BottomSheetState.HalfExpanded;
		});
	}

	async Task BackAsync()
	{
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"..");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}
}
