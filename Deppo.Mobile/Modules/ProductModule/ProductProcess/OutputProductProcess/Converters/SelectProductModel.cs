using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Converters;

public class SelectProductModel
{
	public int ItemReferenceId { get; set; } = default;
	public BottomSheet? BottomSheet { get; set; }
}
