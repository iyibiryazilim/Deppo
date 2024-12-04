using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.OutsourceModels;

public partial class InputOutsourceTransferProductDetailModel : ObservableObject
{
	[ObservableProperty]
	int referenceId;

	[ObservableProperty]
	int seriLotReferenceId;

	[ObservableProperty]
	string seriLotCode = string.Empty;
	
	[ObservableProperty]
	string seriLotName = string.Empty;

	[ObservableProperty]
	int locationReferenceId;

	[ObservableProperty]
	string locationCode = string.Empty;

	[ObservableProperty]
	string locationName = string.Empty;

	[ObservableProperty]
	double quantity;

}
