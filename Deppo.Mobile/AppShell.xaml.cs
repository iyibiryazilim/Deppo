using Deppo.Mobile.Modules.LoginModule.ViewModels;

namespace Deppo.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		var serviceProvider = IPlatformApplication.Current.Services;
		using (var scope = serviceProvider.CreateScope())
		{
			var serviceProviderScoped = scope.ServiceProvider;
			var viewModel = serviceProviderScoped.GetRequiredService<LoginViewModel>();
			BindingContext = viewModel;
		}
	}
}
