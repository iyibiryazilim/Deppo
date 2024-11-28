using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models;

/// <summary>
/// Ambar _ L_CAPIWHOUSE
/// </summary>
public class Warehouse : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private int _number;
    private string _name = string.Empty;
    private string _city = string.Empty;
    private string _country = string.Empty;
    private double _quantity;
    private int _locationCount;

    public Warehouse()
    {

    }

    [Key]
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

    public int Number
    {
        get => _number;
        set
        {
            if (_number == value) return;
            _number = value;
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

    public double Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            NotifyPropertyChanged();
        }
    }

    public int LocationCount
    {
        get => _locationCount;
        set
        {
            if (_locationCount == value) return;
            _locationCount = value;
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
