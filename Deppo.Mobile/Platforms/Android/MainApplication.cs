using Android.App;
using Android.Runtime;

namespace Deppo.Mobile;

[Application]
[assembly: UsesPermission(Android.Manifest.Permission.ReadMediaImages)]
[assembly: UsesFeature("android.hardware.camera", Required = true)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = true)]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
