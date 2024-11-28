using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models;

/// <summary>
/// Sevk / Kabul Adresi - LG_SHIPINFO
/// </summary>
public class ShipAddress : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private string _code = string.Empty;
    private string _name = string.Empty;
    private string _address = string.Empty;
    private string _city = string.Empty;
    private string _country = string.Empty;

    public ShipAddress()
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

    public string Address
    {
        get => _address;
        set
        {
            if (_address == value) return;
            _address = value;
            NotifyPropertyChanged();
        }
    }

    public string City
    {
        get => _city;
        set
        {
            if (_city == value) return;
            _city = value;
            NotifyPropertyChanged();
        }
    }

    public string Country
    {
        get => _country;
        set
        {
            if (_country == value) return;
            _country = value;
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
