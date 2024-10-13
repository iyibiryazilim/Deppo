using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;

public class ProcurementCustomerBasketModel : INotifyPropertyChanged, IDisposable
{
    private int _locationReferenceId;
    private string _locationCode = string.Empty;
    private string _locationName = string.Empty;
    private int _warehouseNumber;
    private string _warehouseName = string.Empty;
    private List<ProcurementCustomerBasketProductModel> _products = new();
    private List<ProcurementCustomerProductModel> _procurementProductList = new();


    public ProcurementCustomerBasketModel()
    {
        Products = new List<ProcurementCustomerBasketProductModel>();
    }

    [Browsable(false)]
    public int LocationReferenceId
    {
        get => _locationReferenceId;
        set
        {
            if (_locationReferenceId == value) return;
            _locationReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Stok Yeri Kodu")]
    public string LocationCode
    {
        get => _locationCode;
        set
        {
            if (_locationCode == value) return;
            _locationCode = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Stok Yeri Adı")]
    public string LocationName
    {
        get => _locationName;
        set
        {
            if (_locationName == value) return;
            _locationName = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Ambar Numarası")]
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

    [DisplayName("Ambar Adı")]
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

    public List<ProcurementCustomerBasketProductModel> Products
    {
        get => _products;
        set
        {
            if (_products == value) return;
            _products = value;
            NotifyPropertyChanged();
        }
    }

    public List<ProcurementCustomerProductModel> ProcurementProductList
    {
        get => _procurementProductList;
        set
        {
            if (_procurementProductList == value) return;
            _procurementProductList = value;
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
