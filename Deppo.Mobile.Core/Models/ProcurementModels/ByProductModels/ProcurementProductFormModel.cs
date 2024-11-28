using CommunityToolkit.Mvvm.ComponentModel;
using Deppo.Mobile.Core.Models.SalesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByProductModels;

public partial class ProcurementProductFormModel : ObservableObject
{
	[ObservableProperty]
	ProcurementProductBasketProductModel? procurementItem;

	[ObservableProperty]
	CustomerOrderModel selectedCustomer;


	[ObservableProperty]
	private DateTime transactionDate = DateTime.Now;
	[ObservableProperty]
	private string documentNumber = string.Empty;
	[ObservableProperty]
	private string specialCode = string.Empty;
	[ObservableProperty]
	private string description = string.Empty;

	

}
