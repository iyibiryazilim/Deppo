using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Mobile.Helpers.MVVMHelper;

namespace Deppo.Mobile.Modules.ResultModule.ViewModels;

[QueryProperty(name: nameof(ResultModel), queryId: nameof(ResultModel))]
public partial class InsertSuccessPageViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	ResultModel resultModel = null!;

	public InsertSuccessPageViewModel(IUserDialogs userDialogs)
	{
		_userDialogs = userDialogs;
		Title = "";

		BackCommand = new Command(async () => await BackAsync());
	}

	public Command BackCommand { get; }

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
