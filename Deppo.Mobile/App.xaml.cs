using Deppo.Mobile.Modules.LoginModule.Views;

namespace Deppo.Mobile;

public partial class App : Application
{
	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();

		MainPage = serviceProvider.GetRequiredService<LoginView>();
	}
}
