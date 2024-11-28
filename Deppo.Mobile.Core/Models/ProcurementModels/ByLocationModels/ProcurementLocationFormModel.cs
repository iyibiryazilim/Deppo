using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.SalesModels;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByLocationModels
{
	public partial class ProcurementLocationFormModel : ObservableObject
	{
		[ObservableProperty]
		ProcurementCustomerModel? procurementCustomerModel;

		[ObservableProperty]
		ObservableCollection<ProcurementByLocationProduct> procurementByLocationProduct = new();

		[ObservableProperty]
		string documentNumber = string.Empty;

		[ObservableProperty]
		DateTime transactionDate = DateTime.Now;

		[ObservableProperty]
		string description = string.Empty;

		[ObservableProperty]
		string documentTrackingNumber = string.Empty;

		[ObservableProperty]
		string specialCode = string.Empty;

		private bool isFormCompleted;
		public bool IsFormCompleted
		{
			get => isFormCompleted;
			set
			{
				SetProperty(ref isFormCompleted, value);
				OnPropertyChanged(nameof(ProcurementStatusText));
				OnPropertyChanged(nameof(ProcurementStatusTextColor));
			}
		}

		private string procurementStatusText = "Bekliyor";
		public string ProcurementStatusText
		{
			get
			{
				
				return IsFormCompleted ? "Tamamlandı" : "Bekliyor";
			}
			set => SetProperty(ref procurementStatusText, value);
		}

		private string procurementStatusTextColor = "#E6BE0C";
		public string ProcurementStatusTextColor
		{
			get
			{
				
				return IsFormCompleted ? "Green" : "#E6BE0C";
			}
			set => SetProperty(ref procurementStatusTextColor, value);
		}

	}
}
