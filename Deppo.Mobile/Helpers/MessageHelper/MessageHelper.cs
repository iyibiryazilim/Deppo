using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Deppo.Mobile.Helpers.MessageHelper;

public class MessageHelper
{
	public IToast GetToastMessage(string message, ToastDuration toastDuration = ToastDuration.Long, double toastFontSize = 14)
	{
		return Toast.Make(message: message, duration: toastDuration, textSize: toastFontSize);
	}
}
