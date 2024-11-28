using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Deppo.Mobile.Core.Models.BasketModels;

public class InputProductBasketDetailModel : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private int _seriLotReferenceId;
    private string _seriLotCode = string.Empty;
    private string _seriLotName = string.Empty;
    private int _locationReferenceId;
    private string _locationCode = string.Empty;
    private string _locationName = string.Empty;
    private double _quantity;

    public InputProductBasketDetailModel()
    {

    }

    [Browsable(false)]
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

    [Browsable(false)]
    public int SeriLotReferenceId
    {
        get => _seriLotReferenceId;
        set
        {
            if (_seriLotReferenceId == value) return;
            _seriLotReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Seri / Lot Kodu")]
    public string SeriLotCode
    {
        get => _seriLotCode;
        set
        {
            if (_seriLotCode == value) return;
            _seriLotCode = value;
            NotifyPropertyChanged();
        }
    }

    [DisplayName("Seri / Lot Adı")]
    public string SeriLotName
    {
        get => _seriLotName;
        set
        {
            if (_seriLotName == value) return;
            _seriLotName = value;
            NotifyPropertyChanged();
        }
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

    [DisplayName("Miktar")]
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
