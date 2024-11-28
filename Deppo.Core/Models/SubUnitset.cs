using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Deppo.Core.Models;

/// <summary>
/// Birim - LG_UNITSETL
/// </summary>
public class SubUnitset : INotifyPropertyChanged, IDisposable
{
    private int _referenceId;
    private int _unitsetReferenceId;
    private Unitset? _unitset;
    private string _code = string.Empty;
    private string _name = string.Empty;
    private double _conversionValue;
    private double _otherConversionValue;

    public SubUnitset()
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

    [Browsable(false)]
    public int UnitsetReferenceId
    {
        get => _unitsetReferenceId;
        set
        {
            if (_unitsetReferenceId == value) return;
            _unitsetReferenceId = value;
            NotifyPropertyChanged();
        }
    }

    public Unitset? Unitset
    {
        get => _unitset;
        set
        {
            if (_unitset == value) return;
            _unitset = value;
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

    public double ConversionValue
    {
        get => _conversionValue;
        set
        {
            if (_conversionValue == value) return;
            _conversionValue = value;
            NotifyPropertyChanged();
        }
    }

    public double OtherConversionValue
    {
        get => _otherConversionValue;
        set
        {
            if (_otherConversionValue == value) return;
            _otherConversionValue = value;
            NotifyPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
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
