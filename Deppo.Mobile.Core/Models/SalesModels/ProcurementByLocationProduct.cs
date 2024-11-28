using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.SalesModels
{
	public partial class ProcurementByLocationProduct : ObservableObject
	{
		[ObservableProperty]
		int itemReferenceId;

		[ObservableProperty]
		string itemCode = string.Empty;

		[ObservableProperty]
		string itemName = string.Empty;

		[ObservableProperty]
		int mainItemReferenceId;

		[ObservableProperty]
		string mainItemCode = string.Empty;

		[ObservableProperty]
		string mainItemName = string.Empty;


		[ObservableProperty]
		int subUnitsetReferenceId;

		[ObservableProperty]
		string subUnitsetName = string.Empty;

		[ObservableProperty]
		string subUnitsetCode = string.Empty;

		[ObservableProperty]
		int unitsetReferenceId;
		[ObservableProperty]
		string unitsetName = string.Empty;
		[ObservableProperty]
		string unitsetCode = string.Empty;

		[ObservableProperty]
		double stockQuantity;

		[ObservableProperty]
		double outputQuantity;

		[ObservableProperty]
		string? image;

		public byte[] ImageData
		{
			get
			{
				if (string.IsNullOrEmpty(Image))
					return Array.Empty<byte>();
				else
				{
					return Convert.FromBase64String(Image);
				}
			}
		}

		[ObservableProperty]
		bool isVariant;

		[ObservableProperty]
		int trackingType;

		[ObservableProperty]
		int locTracking;

		[ObservableProperty]
		bool isSelected;

		[ObservableProperty]
		private string _variantIcon = "bookmark";
		public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

		[ObservableProperty]
		private string _trackingTypeIcon = "box-archive";
		public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

		[ObservableProperty]
		private string _locTrackingIcon = "location-dot";
		public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

		

	}
}
