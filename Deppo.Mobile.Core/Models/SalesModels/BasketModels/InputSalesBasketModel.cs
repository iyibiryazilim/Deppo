using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Mobile.Core.Models.SalesModels.BasketModels;

public class InputSalesBasketModel : INotifyPropertyChanged, IDisposable
{
	private List<InputSalesBasketOrderModel> _details = new();

    public InputSalesBasketModel()
    {
        
    }

	public List<InputSalesBasketOrderModel> Details
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
