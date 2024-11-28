using Android.Content;
using Android.Views.InputMethods;
using CommunityToolkit.Maui.Core.Platform;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;

public partial class OutputProductSalesOrderProcessBasketListView : ContentPage
{
	private readonly OutputProductSalesOrderProcessBasketListViewModel _viewModel;
	public OutputProductSalesOrderProcessBasketListView(OutputProductSalesOrderProcessBasketListViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		_viewModel.CurrentPage = this;
		_viewModel.LocationTransactionSearchText = locationTransactionSearchBar;
		_viewModel.BarcodeEntry = barcodeEntry;
		//barcodeEntry.Focused += barcodeEntry_Focused;
	}

//	protected async override void OnAppearing()
//	{
//		base.OnAppearing();

//		await Task.Delay(500).ContinueWith(t =>
//		{
//			MainThread.InvokeOnMainThreadAsync(() =>
//			{
//				barcodeEntry.Focus();
//			});
//		});

//	}

//	public void HideKeyboard()
//	{
//#if (ANDROID)
//		{
//			var context = Platform.AppContext;
//			var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
//			if (inputMethodManager != null)
//			{
//				var activity = Platform.CurrentActivity;
//				if (activity != null)
//				{
//					var token = activity.CurrentFocus?.WindowToken;
//					inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
//				}

//				//barcodeEntry.HideKeyboardAsync(CancellationToken.None);
//				//barcodeEntry.HideSoftInputAsync(CancellationToken.None);
//				// if (activity?.Window != null)
//				// 	activity.Window.DecorView.ClearFocus();
//			}
//		}
//#endif
//	}

//	private void barcodeEntry_Focused(object? sender, FocusEventArgs e)
//	{
//		if (e.IsFocused)
//		{
//			HideKeyboard();
//		}
//	}
}