using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Converters;

public class SelectProductConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		if (values != null && values.Length == 2)
		{
			int itemReferenceId = values[0] is null ? default : System.Convert.ToInt32(values[0]);
			BottomSheet bottomSheet = values[1] as BottomSheet;

			return new SelectProductModel { ItemReferenceId = itemReferenceId, BottomSheet = bottomSheet };
		}
		return null;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
