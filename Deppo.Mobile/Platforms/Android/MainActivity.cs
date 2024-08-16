using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Deppo.Mobile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);

		#region Disable dark mode
		var disableDarkMode = (UiModeManager)GetSystemService(UiModeService);
		disableDarkMode.SetApplicationNightMode(1);
		#endregion
		Android.Views.View decorView = Window.DecorView;
		decorView.SystemUiVisibility = (StatusBarVisibility)(
			SystemUiFlags.HideNavigation |
			SystemUiFlags.Fullscreen |
			SystemUiFlags.ImmersiveSticky);
	}
}
