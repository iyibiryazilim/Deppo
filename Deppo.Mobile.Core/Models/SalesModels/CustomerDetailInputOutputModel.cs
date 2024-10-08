using CommunityToolkit.Mvvm.ComponentModel;

namespace Deppo.Mobile.Core.Models.SalesModels;

public class CustomerDetailInputOutputModel : ObservableObject
{
	string argument = string.Empty;
	int argumentDay = 0;
	double inputQuantity = 0;
	double outputQuantity = 0;

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

	public double InputQuantity
	{
		get => inputQuantity;
		set => SetProperty(ref inputQuantity, value);
	}

	public double OutputQuantity
	{
		get => outputQuantity;
		set => SetProperty(ref outputQuantity, value);
	}
}
