using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByLocationModels
{
	public partial class ProcurementCustomerModel : ObservableObject
	{
		[ObservableProperty]
		int referenceId;

		[ObservableProperty]
		int orderReferenceId;

		[ObservableProperty]
		string orderNumber = string.Empty;

		[ObservableProperty]
		int customerReferenceId;

		[ObservableProperty]
		string customerCode = string.Empty;

		[ObservableProperty]
		string customerName = string.Empty;

		[ObservableProperty]
		string city = string.Empty;

		[ObservableProperty]
		string country = string.Empty;

		[ObservableProperty]
		int productReferenceId;

		[ObservableProperty]
		string productCode = string.Empty;

		[ObservableProperty]
		string productName = string.Empty;

		[ObservableProperty]
		double quantity;

		[ObservableProperty]
		double shippedQuantity;

		[ObservableProperty]
		ShipAddress shipAddress;

		[ObservableProperty]
		int shipAddressCount;

		private double waitingQuantity;
		public double WaitingQuantity
		{
			get => waitingQuantity;
			set
			{
				SetProperty(ref waitingQuantity, value);
				OnPropertyChanged(nameof(ProcurementStatusText));
				OnPropertyChanged(nameof(ProcurementStatusTextColor));
			}
		}

		[ObservableProperty]
		DateTime orderDate;

		[ObservableProperty]
		DateTime dueDate;

		private double outputQuantity;
		public double OutputQuantity
		{
			get => outputQuantity;
			set
			{
				SetProperty(ref outputQuantity, value);
				OnPropertyChanged(nameof(ProcurementStatusText));
				OnPropertyChanged(nameof(ProcurementStatusTextColor));
			}
		}

		public string TitleName => CustomerName?.Length > 2 ? CustomerName.Substring(0, 2) : CustomerName;

		[ObservableProperty]
		Guid rejectionOid = Guid.Empty;

		[ObservableProperty]
		string rejectionCode = string.Empty;

		[ObservableProperty]
		string rejectionName = string.Empty;

		private string _procurementStatusText = "Bekliyor";

		private string procurementStatusText = "Bekliyor";
		public string ProcurementStatusText
		{
			get
			{
				if (!string.IsNullOrEmpty(RejectionCode))
				{
					return "Hata";
				}
				return OutputQuantity == WaitingQuantity ? "Tamamlandı" : "Bekliyor";
			}
			set => SetProperty(ref procurementStatusText, value);
		}

		private string procurementStatusTextColor = "#E6BE0C";
		public string ProcurementStatusTextColor
		{
			get
			{
				if (!string.IsNullOrEmpty(RejectionCode))
				{
					return "Red";
				}
				return OutputQuantity == WaitingQuantity ? "Green" : "#E6BE0C";
			}
			set => SetProperty(ref procurementStatusTextColor, value);
		}

	}
}
