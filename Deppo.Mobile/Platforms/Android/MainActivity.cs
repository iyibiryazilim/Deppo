using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace Deppo.Mobile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
     // protected override void OnCreate(Bundle savedInstanceState)
     // {
     // 	base.OnCreate(savedInstanceState);

     // 	#region Disable dark mode
     // 	var disableDarkMode = (UiModeManager)GetSystemService(UiModeService);
     // 	disableDarkMode.SetApplicationNightMode(1);
     // 	#endregion
     // 	Android.Views.View decorView = Window.DecorView;
     // 	decorView.SystemUiVisibility = (StatusBarVisibility)(
     // 		SystemUiFlags.HideNavigation |
     // 		SystemUiFlags.Fullscreen |
     // 		SystemUiFlags.ImmersiveSticky);
     // }

     protected override void OnCreate(Bundle? savedInstanceState)
     {
        base.OnCreate(savedInstanceState);
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.R)
        {
            Window.SetDecorFitsSystemWindows(false);
            var windowInsetsController = Window.DecorView.WindowInsetsController;
            if (windowInsetsController != null)
            {
                windowInsetsController.Hide(WindowInsetsCompat.Type.NavigationBars() | WindowInsetsCompat.Type.StatusBars());
            }
        }

		var uiOptions = SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky; // hide navigation bar
		Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;


		if (DeviceInfo.Idiom == DeviceIdiom.Phone)
		{
			RequestedOrientation = ScreenOrientation.Portrait;
		}
	}
}
