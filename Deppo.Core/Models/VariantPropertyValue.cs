using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Models;

public class VariantPropertyValue : INotifyPropertyChanged, IDisposable
{

    private int _referenceId;
    private string _code = string.Empty;
    private string _name = string.Empty;
    private int _variantPropertyReferenceId;
    private int _valNo;

    public VariantPropertyValue()
    {

    }
    public int ReferenceId
    {
        get => _referenceId;
        set
        {
            if (_referenceId == value) return;
            _referenceId = value;
            NotifyPropertyChanged();
        }
    }

   

    public string Code
    {
        get => _code;
        set
        {
            if (_code == value) return;
            _code = value;
            NotifyPropertyChanged();
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name == value) return;
            _name = value;
            NotifyPropertyChanged();
        }
    }
    public int VariantPropertyReferenceId
    {
        get => _variantPropertyReferenceId;
        set
        {
            if (_variantPropertyReferenceId == value) return;
            _variantPropertyReferenceId = value;
            NotifyPropertyChanged();
        }
    }
    public int ValNo
    {
        get => _valNo;
        set
        {
            if (_valNo == value) return;
            _valNo = value;
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
