using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Sys.Service.Models;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.AnalysisModels
{
    public partial class PerformanceAnalysisModel : ObservableObject
    {
        [ObservableProperty]
        public ApplicationUser? applicationUser;

        [ObservableProperty]
        int transactionCount;

		[ObservableProperty]
        bool iconVisibility;

        [ObservableProperty]
        string icon = "medal";

        [ObservableProperty]
        string iconColor = string.Empty;
	}
}
