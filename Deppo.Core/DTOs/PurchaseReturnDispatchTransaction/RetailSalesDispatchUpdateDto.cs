namespace Deppo.Core.DTOs.PurchaseReturnDispatchTransaction
{
	public class RetailSalesDispatchUpdateDto
	{
		public int FicheReferenceId { get; set; }
		public string? CarrierCode { get; set; } = string.Empty;
		public string? DriverFirstName { get; set; } = string.Empty;
		public string? DriverLastName { get; set; } = string.Empty;
		public string? IdentityNumber { get; set; } = string.Empty;
		public string? Plaque { get; set; } = string.Empty;
		public string? DocumentNumber { get; set; } = string.Empty;
		public DateTime TransactionDate { get; set; } = DateTime.Now;
		public string? DocumentTrackingNumber { get; set; } = string.Empty;
		public string? Description { get; set; } = string.Empty;
		public string? SpeCode { get; set; } = string.Empty;
		public int IsEDispatch { get; set; }
		public int FirmNumber { get; set; }
	}
}
