using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;

public class InputPurchaseBasketOrderModel : INotifyPropertyChanged, IDisposable
{
    private List<InputPurchaseBasketOrderModel> _details = new();

    public InputPurchaseBasketOrderModel()
    {
    }

    public List<InputPurchaseBasketOrderModel> Details
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