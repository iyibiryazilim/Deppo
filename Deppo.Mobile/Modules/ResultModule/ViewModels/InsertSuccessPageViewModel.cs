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

			if(ResultModel.PageCountToBack != 0)
			{
				switch (ResultModel.PageCountToBack)
				{
					case 1:
						await Shell.Current.GoToAsync("..");
						break;
					case 2:
						await Shell.Current.GoToAsync("../..");
						break;
					case 3:
						await Shell.Current.GoToAsync("../../..");
						break;
					case 4:
						await Shell.Current.GoToAsync("../../../..");
						break;
					case 5:
						await Shell.Current.GoToAsync("../../../../..");
						break;
					case 6:
						await Shell.Current.GoToAsync("../../../../../..");
						break;
                    case 7:
                        await Shell.Current.GoToAsync("../../../../../../..");
                        break;
                    case 8:
                        await Shell.Current.GoToAsync("../../../../../../../..");
                        break;
					case 9:
						await Shell.Current.GoToAsync("../../../../../../../../..");
						break;
					case 10:
						await Shell.Current.GoToAsync("../../../../../../../../../..");
						break;
					case 11:
						await Shell.Current.GoToAsync("../../../../../../../../../../..");
						break;
					default:
						await Shell.Current.GoToAsync("..");
						break;
				}
			} 
			else
			{
				await Shell.Current.GoToAsync("..");
			}

			
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
