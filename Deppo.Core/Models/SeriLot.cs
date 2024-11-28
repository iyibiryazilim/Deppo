using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Core.Models;

public class SeriLot : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private string _code = string.Empty;
    private string _name = string.Empty;
    private int _warehouseReferenceId;
    private int _warehouseNumber;
    private string _warehouseName = string.Empty;
    private double _stockQuantity;

    public SeriLot()
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

    public int WarehouseReferenceId
    {
        get => _warehouseReferenceId;
        set
        {
            if (_warehouseReferenceId == value) return;
            _warehouseReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public int WarehouseNumber
    {
        get => _warehouseNumber;
        set
        {
            if (_warehouseNumber == value) return;
            _warehouseNumber = value;
            NotifyPropertyChanged();
        }
    }

    public string WarehouseName
    {
        get => _warehouseName;
        set
        {
            if (_warehouseName == value) return;
            _warehouseName = value;
            NotifyPropertyChanged();
        }
    }
    

    public double StockQuantity
    {
        get => _stockQuantity;
        set
        {
            if (_stockQuantity == value) return;
            _stockQuantity = value;
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
