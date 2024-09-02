using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Mobile.Core.Models.SalesModels.BasketModels;

public class OutputSalesBasketModel : INotifyPropertyChanged, IDisposable
{

	private List<OutputSalesBasketOrderModel> _details = new();

	public OutputSalesBasketModel()
	{

	}

	public List<OutputSalesBasketOrderModel> Details
	{
		get => _details;
		set
		{
			if (_details == value) return;
			_details = value;
			NotifyPropertyChanged();
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);

	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			PropertyChanged = null;
		}
	}
}
