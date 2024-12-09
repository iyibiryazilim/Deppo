using Deppo.Mobile.Modules.LoginModule.Views;

namespace Deppo.Mobile;

public partial class App : Application
{
	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH1eeXRQRWRZVEJ/V0I=");
		MainPage = serviceProvider.GetRequiredService<LoginView>();
	}
}
