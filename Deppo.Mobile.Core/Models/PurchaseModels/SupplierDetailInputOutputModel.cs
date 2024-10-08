using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.PurchaseModels;

public class SupplierDetailInputOutputModel : ObservableObject
{
	string argument = string.Empty;
	int argumentDay = 0;
	double purchaseReferenceQuantity= 0;
	double returnReferenceQuantity = 0;

	public SupplierDetailInputOutputModel()
	{

	}

	public string Argument
	{
		get => argument;
		set => SetProperty(ref argument, value);
	}

	public int ArgumentDay
	{
		get => argumentDay;
		set => SetProperty(ref argumentDay, value);
	}

	public double PurchaseReferenceQuantity
	{
		get => purchaseReferenceQuantity;
		set => SetProperty(ref purchaseReferenceQuantity, value);
	}

	public double ReturnReferenceQuantity
	{
		get => returnReferenceQuantity;
		set => SetProperty(ref returnReferenceQuantity, value);
	}	
}
