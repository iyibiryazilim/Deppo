using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.CameraModels;

public partial class CameraScanModel : ObservableObject
{
	[ObservableProperty]
	string comingPage = string.Empty;

	[ObservableProperty]
	int warehouseNumber;

	[ObservableProperty]
	int currentReferenceId = 0;

	[ObservableProperty]
	int shipInfoReferenceId = 0;
}
