using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.SalesModels;

public class CustomerDetailInputOutputModel : ObservableObject
{
	string argument = string.Empty;
	int argumentDay = 0;
	double salesReferenceQuantity = 0;
	double returnReferenceQuantity = 0;

	public CustomerDetailInputOutputModel()
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

	public double SalesReferenceQuantity
	{
		get => salesReferenceQuantity;
		set => SetProperty(ref salesReferenceQuantity, value);
	}

	public double ReturnReferenceQuantity
	{
		get => returnReferenceQuantity;
		set => SetProperty(ref returnReferenceQuantity, value);
	}
}
