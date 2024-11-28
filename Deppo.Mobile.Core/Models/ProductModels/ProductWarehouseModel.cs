using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;

namespace Deppo.Mobile.Core.Models.ProductModels;

public class ProductWarehouseModel : INotifyPropertyChanged,IDisposable
{
    private int _productReferenceId;
    private Product _product = null!;
    private int _warehouseReferenceId;
    private Warehouse _warehouse = null!;
    private double _stockQuantity;

    public ProductWarehouseModel()
    {
    }

    [Browsable(false)]
    public int ProductReferenceId
    {
        get => _productReferenceId;
        set
        {
            if (_productReferenceId == value) return;
            _productReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public Product Product
    {
        get => _product;
        set
        {
            if (_product == value) return;
            _product = value;
            NotifyPropertyChanged();
        }
    }

    [Browsable(false)]
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

    public Warehouse Warehouse
    {
        get => _warehouse;
        set
        {
            if (_warehouse == value) return;
            _warehouse = value;
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
