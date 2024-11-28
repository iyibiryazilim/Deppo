using Deppo.Mobile.Modules.LoginModule.Views;

namespace Deppo.Mobile;

public partial class App : Application
{
	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpTXxbf1x0ZF1MZF5bRHZPMyBoS35RckRiWXdeeHBWQmlVVkZ+");
		MainPage = serviceProvider.GetRequiredService<LoginView>();
	}
}
