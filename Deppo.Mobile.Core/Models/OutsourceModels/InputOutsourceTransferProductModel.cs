using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels;

public partial class InputOutsourceTransferProductModel : ObservableObject
{
	[ObservableProperty]
	int referenceId = default;

	[ObservableProperty]
	int productionReferenceId = default;

	[ObservableProperty]
	int productReferenceId = default;

	[ObservableProperty]
	string productCode = string.Empty;

	[ObservableProperty]
	string productName = string.Empty;

	[ObservableProperty]
	int unitsetReferenceId = default;

	[ObservableProperty]
	string unitsetCode = string.Empty;

	[ObservableProperty]
	string unitsetName = string.Empty;

	[ObservableProperty]
	int subUnitsetReferenceId = default;

	[ObservableProperty]
	string subUnitsetCode = string.Empty;

	[ObservableProperty]
	string subUnitsetName = string.Empty;

	[ObservableProperty]
	int outsourceReferenceId = default;

	[ObservableProperty]
	string outsourceCode = string.Empty;

	[ObservableProperty]
	string outsourceName = string.Empty;

	[ObservableProperty]
	int operationReferenceId = default;

	[ObservableProperty]
	string operationCode = string.Empty;

	[ObservableProperty]
	string operationName = string.Empty;

	[ObservableProperty]	
	double planningQuantity = default;

	[ObservableProperty]
	double actualQuantity = default;

	[ObservableProperty]
	double stockQuantity = default;

	[ObservableProperty]
	string image = string.Empty;

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
	bool isSelected = false;

	public string LocTrackingIcon => "location-dot";
	public string LocTrackingIconColor => LocTracking == 1 ? "#F5004F" : "#C8C8C8";

	public string VariantIcon => "bookmark";
	public string VariantIconColor => IsVariant ? "#F5004F" : "#C8C8C8";

	public string TrackingTypeIcon => "box-archive";
	public string TrackingTypeIconColor => TrackingType == 1 ? "#F5004F" : "#C8C8C8";

	[ObservableProperty]
	double inputQuantity = default;

	public List<InputOutsourceTransferProductDetailModel> Details { get; set; } = new();

}
